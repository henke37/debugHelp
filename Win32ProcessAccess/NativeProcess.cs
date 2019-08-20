using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Security.Permissions;

namespace Henke37.DebugHelp.Win32 {
	public class NativeProcess : IDisposable {
		internal SafeProcessHandle handle;
		private const int ERROR_BAD_LENGTH = 24;

		internal NativeProcess(SafeProcessHandle handle) {
			if(handle.IsInvalid) throw new ArgumentException("Handle must be valid!", nameof(handle));
			this.handle = handle;
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static NativeProcess Open(uint processId, ProcessAccessRights rights = ProcessAccessRights.All, bool inheritable = false) {
			SafeProcessHandle handle = OpenProcess((uint)rights, inheritable, processId);
			if(handle.IsInvalid) throw new Win32Exception();
			return new NativeProcess(handle);
		}

		public void Dispose() => handle.Dispose();
		public void Close() => handle.Close();

		public UInt32 ProcessId {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				return GetProcessId(handle);
			}
		}

		public UInt32 ExitCode {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var success = GetExitCodeProcess(handle, out uint exitCode);
				if(!success) throw new Win32Exception();
				return exitCode;
			}
		}

		public bool HasExited {
			get {
				var res = WaitForSingleObject(handle, 0);
				if(res == 0) return true;
				if(res == 0x00000102) return false;
				throw new Win32Exception();
			}
		}

		public UInt32 GdiObjectsCurrent => GetGuiResources(handle, 0);
		public UInt32 GdiObjectsPeak => GetGuiResources(handle, 2);
		public UInt32 UserObjectsCurrent => GetGuiResources(handle, 1);
		public UInt32 UserObjectsPeak => GetGuiResources(handle, 4);

		public UInt32 HandleCount {
			get {
				var success = GetProcessHandleCount(handle, out UInt32 count);
				if(!success) throw new Win32Exception();
				return count;
			}
		}

		public bool DynamicPriorityBoosts {
			get {
				var success = GetProcessPriorityBoost(handle, out var disableBoosts);
				if(!success) throw new Win32Exception();
				return !disableBoosts;
			}
		}

		public bool IsWow64Process {
			get {
				var success = IsWow64ProcessNative(handle, out var status);
				if(!success) throw new Win32Exception();
				return status;
			}
		}

		public bool IsBeingDebugged {
			get {
				var success = CheckRemoteDebuggerPresent(handle, out var status);
				if(!success) throw new Win32Exception();
				return status;
			}
		}

		public ProcessTimes GetProcessTimes() {
			var success = GetProcessTimesNative(handle, out var creationTime, out var exitTime, out var kernelTime, out var userTime);
			if(!success) throw new Win32Exception();
			return new ProcessTimes() {
				CreationTime = FiletimeToDateTime(creationTime),
				ExitTime = FiletimeToDateTime(exitTime),
				KernelTime = FiletimeToTimeSpan(kernelTime),
				UserTime = FiletimeToTimeSpan(userTime)
			};
		}

		public UInt64 ProcessorAffinityMask {
			get {
				var success = GetProcessAffinityMask(handle, out var procMask, out var _);
				if(!success) throw new Win32Exception();
				return procMask;
			}
		}

		public IOCounters GetIoCounters() {
			var success = GetProcessIoCounters(handle, out var counters);
			if(!success) throw new Win32Exception();
			return counters;
		}

#if x86
		public WorkingSetBlock[] QueryWorkingSet() {
			int numEntries = 1;
			int headerSize = Marshal.SizeOf<UInt32>();
			int blockSize = Marshal.SizeOf<UInt32>();
			int buffSize = headerSize + blockSize * numEntries;
			IntPtr buffer = Marshal.AllocHGlobal(buffSize);
			for(; ; ) {
				Marshal.WriteInt32(buffer, numEntries);
				var success = QueryWorkingSetNative(handle, buffer, buffSize);
				if(success) break;
				var err = Marshal.GetLastWin32Error();
				if(err != ERROR_BAD_LENGTH) throw new Win32Exception(err);
				numEntries = Marshal.ReadInt32(buffer);
				buffSize = headerSize + blockSize * numEntries;
				buffer = Marshal.ReAllocHGlobal(buffer, (IntPtr)buffSize);
			}
			numEntries = Marshal.ReadInt32(buffer);
			WorkingSetBlock[] blocks = new WorkingSetBlock[numEntries];
			for(int blockIndex = 0; blockIndex < numEntries; ++blockIndex) {
				blocks[blockIndex] = new WorkingSetBlock(
					Marshal.ReadInt32(buffer + headerSize + blockSize * blockIndex)
				);
			}
			Marshal.FreeHGlobal(buffer);
			return blocks;
		}
#endif

		public void ReprotectMemory(IntPtr baseAddr, int size, MemoryProtection newProtection, out MemoryProtection oldProtection) {
			bool success = VirtualProtectEx(handle, baseAddr, size, (uint)newProtection, out var old);
			if(!success) throw new Win32Exception();
			oldProtection = (MemoryProtection)old;
		}

		public IntPtr AllocateMemory(IntPtr newAddr, int size, MemoryAllocationType allocationType, MemoryProtection newProtection) {
			newAddr = VirtualAllocEx(handle, newAddr, size, (uint)allocationType, (uint)newProtection);
			if(newAddr == IntPtr.Zero) throw new Win32Exception();
			return newAddr;
		}

		public void DeallocateMemory(IntPtr oldAddr, int size, MemoryDeallocationType deallocationType) {
			bool success = VirtualFreeEx(handle, oldAddr, size, (uint)deallocationType);
			if(!success) throw new Win32Exception();
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Terminate(UInt32 exitCode) {
			bool success = TerminateProcess(handle, exitCode);
			if(!success) throw new Win32Exception();
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern SafeProcessHandle OpenProcess(UInt32 access, [MarshalAs(UnmanagedType.Bool)] bool inheritable, UInt32 processId);

		public static NativeProcess FromProcess(Process stdProcess) {
			return new NativeProcess(new SafeProcessHandle(DuplicateHandleLocal(stdProcess.Handle, 0, false, DuplicateOptions.SameAccess), true));
		}
		public static NativeProcess FromProcess(Process stdProcess, ProcessAccessRights accessRights) {
			return new NativeProcess(new SafeProcessHandle(DuplicateHandleLocal(stdProcess.Handle, (uint)accessRights, false, DuplicateOptions.None), true));
		}

		public static implicit operator NativeProcess(Process stdProcess) => FromProcess(stdProcess);

		[SecurityCritical]
		internal static unsafe IntPtr DuplicateHandleLocal(IntPtr sourceHandle, uint desiredAccess, bool inheritHandle, DuplicateOptions options) {
			IntPtr newHandle = IntPtr.Zero;
			bool success = DuplicateHandle(SafeProcessHandle.CurrentProcess, sourceHandle, SafeProcessHandle.CurrentProcess, (IntPtr)(int)&newHandle, desiredAccess, inheritHandle, options);

			if(!success) throw new Win32Exception();
			return newHandle;
		}

		[Flags]
		public enum DuplicateOptions : uint {
			None = 0,
			CloseSource = 0x00000001,// Closes the source handle. This occurs regardless of any error status returned.
			SameAccess = 0x00000002, //Ignores the dwDesiredAccess parameter. The duplicate handle has the same access as the source handle.
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DuplicateHandle(SafeProcessHandle sourceProcess, IntPtr sourceHandle, SafeProcessHandle destinationProcess, IntPtr destinationHandlePtr, uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, DuplicateOptions dwOptions);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern UInt32 GetProcessId(SafeProcessHandle handle);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetExitCodeProcess(SafeProcessHandle handle, out UInt32 exitCode);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool TerminateProcess(SafeProcessHandle handle, UInt32 exitCode);

		[DllImport("Ntdll.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern unsafe bool NtQueryInformationProcess(SafeProcessHandle handle, ProcessInformationClass informationClass, void* buffer, uint bufferLength, out uint returnLength);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern UInt32 WaitForSingleObject(SafeProcessHandle handle, UInt32 timeout);

		[DllImport("User32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern UInt32 GetGuiResources(SafeProcessHandle handle, UInt32 flags);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetProcessHandleCount(SafeProcessHandle handle, out UInt32 exitCode);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetProcessPriorityBoost(SafeProcessHandle handle, [MarshalAs(UnmanagedType.Bool)] out bool pDisablePriorityBoost);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "IsWow64Process")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool IsWow64ProcessNative(SafeProcessHandle handle, [MarshalAs(UnmanagedType.Bool)] out bool pDisablePriorityBoost);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CheckRemoteDebuggerPresent(SafeProcessHandle handle, [MarshalAs(UnmanagedType.Bool)] out bool pDisablePriorityBoost);

		[DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetProcessTimes")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetProcessTimesNative(SafeProcessHandle hProcess,
			out System.Runtime.InteropServices.ComTypes.FILETIME lpCreationTime,
			out System.Runtime.InteropServices.ComTypes.FILETIME lpExitTime,
			out System.Runtime.InteropServices.ComTypes.FILETIME lpKernelTime,
			out System.Runtime.InteropServices.ComTypes.FILETIME lpUserTime
		);

		public static DateTime FiletimeToDateTime(System.Runtime.InteropServices.ComTypes.FILETIME fileTime) {
			//NB! uint conversion must be done on both fields before ulong conversion
			ulong hFT2 = unchecked((((ulong)(uint)fileTime.dwHighDateTime) << 32) | (uint)fileTime.dwLowDateTime);
			return DateTime.FromFileTimeUtc((long)hFT2);
		}

		public static TimeSpan FiletimeToTimeSpan(System.Runtime.InteropServices.ComTypes.FILETIME fileTime) {
			//NB! uint conversion must be done on both fields before ulong conversion
			ulong hFT2 = unchecked((((ulong)(uint)fileTime.dwHighDateTime) << 32) | (uint)fileTime.dwLowDateTime);
			return TimeSpan.FromTicks((long)hFT2);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetProcessAffinityMask(SafeProcessHandle hProcess,
			out UInt64 lpProcessAffinityMask,
			out UInt64 lpSystemAffinityMask
		);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "GetProcessIoCounters")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetProcessIoCounters(SafeProcessHandle handle, out IOCounters counters);

		[DllImport("Psapi.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "QueryWorkingSet")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static unsafe extern bool QueryWorkingSetNative(SafeProcessHandle handle, IntPtr pv, int cb);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool VirtualProtectEx(SafeProcessHandle hProcess, IntPtr lpAddress, Int32 dwSize, UInt32 flNewProtect, out UInt32 lpflOldProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr VirtualAllocEx(SafeProcessHandle hProcess, IntPtr lpAddress, Int32 dwSize, UInt32 flAllocationType, UInt32 flProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool VirtualFreeEx(SafeProcessHandle hProcess, IntPtr lpAddress, Int32 dwSize, UInt32 dwFreeType);

	}
}
