using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Security.Principal;

using Henke37.Win32.AccessRights;
using Henke37.Win32.SafeHandles;
using Henke37.Win32.Memory;
using Henke37.Win32.FileMappings;
using Henke37.Win32.Tokens;
using Henke37.Win32.Snapshots;
using Henke37.Win32.Threads;

namespace Henke37.Win32.Processes {
#if NETFRAMEWORK
	[HostProtection(ExternalProcessMgmt = true)]
#endif
	public class NativeProcess : IDisposable {
		internal SafeProcessHandle handle;
		private const int ERROR_BAD_LENGTH = 24;
		private const int WaitForSingleObjectTimeOut = 0x00000102;

		public const CreateProcessFlags SupportedCreateProcessFlags = CreateProcessFlags.BreakAwayFromJob |
			CreateProcessFlags.CreateNewConsole | CreateProcessFlags.CreateNewProcessGroup |
			CreateProcessFlags.CreateNoWindow | CreateProcessFlags.DefaultErrorMode |
			CreateProcessFlags.DetachedProcess | CreateProcessFlags.InheritParentAffinity |
			CreateProcessFlags.PreserveCodeAuthZLevel | CreateProcessFlags.Suspended;
		public const StartupInfoFlags SupportedStartupInfoFlags = StartupInfoFlags.ForceOffFeedback | StartupInfoFlags.ForceOnFeedback |
			StartupInfoFlags.PreventPinning | StartupInfoFlags.TitleIsAppId | StartupInfoFlags.TitleIsLinkName |
			StartupInfoFlags.UntrustedSource |
			StartupInfoFlags.UseCountChars | StartupInfoFlags.UseFillAttribute | StartupInfoFlags.UseHotKey |
			StartupInfoFlags.UsePosition | StartupInfoFlags.UseShowWindow | StartupInfoFlags.UseSize | StartupInfoFlags.UseSTDHandles;

		public static NativeProcess CreateProcess(string applicationName, string commandLine, CreateProcessFlags flags, StartupInfoFlags flags2, string currentDirectory, out NativeThread firstThread) {
			StartupInfoW startupInfo = new StartupInfoW(flags2);
			return CreateProcess(applicationName, commandLine, flags, startupInfo, currentDirectory, out firstThread);
		}

#if NETFRAMEWORK
		[HostProtection(MayLeakOnAbort = true)]
#endif
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static unsafe NativeProcess CreateProcess(string applicationName, string commandLine, CreateProcessFlags flags, StartupInfoW startupInfo, string currentDirectory, out NativeThread firstThread) {
			if((flags & ~SupportedCreateProcessFlags) != 0) {
				throw new ArgumentException("Unsupported CreateProcessFlags given!");
			}
			if((startupInfo.dwFlags & ~SupportedStartupInfoFlags) != 0) {
				throw new ArgumentException("Unsupported StartupInfoFlags given");
			}

			ProcessInformation processInfo;

			bool success = CreateProcessW(applicationName, commandLine, null, null, false, (UInt32)flags, null, currentDirectory, &startupInfo, &processInfo);
			if(!success) throw new Win32Exception();

			firstThread = new NativeThread(new SafeThreadHandle(processInfo.hThread));
			return new NativeProcess(new SafeProcessHandle(processInfo.hProcess));
		}

		internal NativeProcess(SafeProcessHandle handle) {
			if(handle.IsInvalid) throw new ArgumentException("Handle must be valid!", nameof(handle));
			this.handle = handle;
		}

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static NativeProcess Open(uint processId, ProcessAccessRights rights = ProcessAccessRights.All, bool inheritable = false) {
			SafeProcessHandle handle = OpenProcess((uint)rights, inheritable, processId);
			if(handle.IsInvalid) throw new Win32Exception();
			return new NativeProcess(handle);
		}

		private static NativeProcess? CachedCurrent;
		public static NativeProcess Current {
			get {
				if(CachedCurrent != null) return CachedCurrent;
				CachedCurrent = new NativeProcess(SafeProcessHandle.CurrentProcess);
				return CachedCurrent;
			}
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
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var res = WaitForSingleObject(handle, 0);
				if(res == 0) return true;
				if(res == WaitForSingleObjectTimeOut) return false;
				throw new Win32Exception();
			}
		}

		public UInt32 GdiObjectsCurrent => GetGuiResources(handle, 0);
		public UInt32 GdiObjectsPeak => GetGuiResources(handle, 2);
		public UInt32 UserObjectsCurrent => GetGuiResources(handle, 1);
		public UInt32 UserObjectsPeak => GetGuiResources(handle, 4);

		public UInt32 HandleCount {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var success = GetProcessHandleCount(handle, out UInt32 count);
				if(!success) throw new Win32Exception();
				return count;
			}
		}

		public bool DynamicPriorityBoosts {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var success = GetProcessPriorityBoost(handle, out var disableBoosts);
				if(!success) throw new Win32Exception();
				return !disableBoosts;
			}
		}

		public bool IsWow64Process {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var success = IsWow64ProcessNative(handle, out var status);
				if(!success) throw new Win32Exception();
				return status;
			}
		}

		public bool IsBeingDebugged {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var success = CheckRemoteDebuggerPresent(handle, out var status);
				if(!success) throw new Win32Exception();
				return status;
			}
		}

		public bool IsImmersiveProcess {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				return IsImmersiveProcessNative(handle);
			}
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
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
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var success = GetProcessAffinityMask(handle, out var procMask, out var _);
				if(!success) throw new Win32Exception();
				return procMask;
			}
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public IOCounters GetIoCounters() {
			var success = GetProcessIoCounters(handle, out var counters);
			if(!success) throw new Win32Exception();
			return counters;
		}

#if x86
#if NETFRAMEWORK
		[HostProtection(MayLeakOnAbort = true)]
		[ReliabilityContract(Consistency.MayCorruptProcess, Cer.None)]
#endif
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
		public IList<MemoryBasicInformation> QueryMemoryRangeInformation(UIntPtr baseAddress, int size) {
			var ret = new List<MemoryBasicInformation>();
			UIntPtr endAdd = baseAddress + size;
			for(; ; ) {
				MemoryBasicInformation.Native native;
				var result = VirtualQueryEx(handle, baseAddress, out native, (uint)Marshal.SizeOf<MemoryBasicInformation.Native>());
				if(result == 0) throw new Win32Exception();
				ret.Add(native.AsManaged());
				baseAddress += (int)native.RegionSize;
				if((long)baseAddress >= (long)endAdd) break;
			}
			return ret;
		}

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
		public MemoryInfo GetMemoryInfo() {
			MemoryInfo.Native native = new MemoryInfo.Native();
			native.cb = (uint)Marshal.SizeOf<MemoryInfo.Native>();

			bool success = GetProcessMemoryInfo(handle, ref native, native.cb);
			if(!success) throw new Win32Exception();
			return native.AsManaged();
		}

		public string FullImageName {
			get {
				UInt32 size = 200;
				var sb = new StringBuilder((int)size);
				var success = QueryFullProcessImageName(handle, 0, sb, ref size);
				if(!success) throw new Win32Exception();
				return sb.ToString();
			}
		}

		public bool IsCritical {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var success = IsProcessCritical(handle, out bool critical);
				if(!success) throw new Win32Exception();
				return critical;
			}
		}

		public IEnumerable<ThreadEntry> GetThreads() {
			using(var snap = new Toolhelp32Snapshot(Toolhelp32SnapshotFlags.Thread, ProcessId)) {
				//using yield explicitly to avoid `snap` being disposed early
				foreach(var thread in snap.GetThreads()) {
					yield return thread;
				}
			}
		}

		public IEnumerable<ModuleEntry> GetModules() {
			using(var snap = new Toolhelp32Snapshot(Toolhelp32SnapshotFlags.Module, ProcessId)) {
				//using yield explicitly to avoid `snap` being disposed early
				foreach(var module in snap.GetModules()) {
					yield return module;
				}
			}
		}

		public void FlushInstructionCache(IntPtr baseAddr, uint size) {
			var success = FlushInstructionCacheNative(handle, baseAddr, size);
			if(!success) throw new Win32Exception();
		}

		public IntPtr PebBaseAddress {
			get {
				ProcessBasicInformation info = new ProcessBasicInformation();
				QueryInformationProcess<ProcessBasicInformation>(ProcessInformationClass.BasicInformation, ref info);
				return info.PebBaseAddress;
			}
		}

		internal unsafe T QueryInformationProcess<T>(ProcessInformationClass infoClass, ref T buff) where T : unmanaged {
			fixed (void* buffP = &buff) {
				PInvoke.NTSTATUS status = NtQueryInformationProcess(handle, infoClass, buffP, (uint)sizeof(T), out _);
				if(status.Severity != PInvoke.NTSTATUS.SeverityCode.STATUS_SEVERITY_SUCCESS) throw new PInvoke.NTStatusException(status);
				return buff;
			}
		}

		public IntPtr EncodePointer(IntPtr plain) {
			var success = EncodeRemotePointer(handle, plain, out var crypto);
			if(success.Failed) throw new Win32Exception(success);
			return crypto;
		}

		public IntPtr DecodePointer(IntPtr crypto) {
			var success = DecodeRemotePointer(handle, crypto, out var plain);
			if(success.Failed) throw new Win32Exception(success);
			return plain;
		}

		public ProcessWorkingSetSize WorkingSetSize {
			get {
				bool success = GetProcessWorkingSetSizeEx(handle, out uint min, out uint max, out uint flags);
				if(!success) throw new Win32Exception();
				return new ProcessWorkingSetSize(min, max, flags);
			}
		}

		public IntPtr MapFileView(FileMapping fileMapping, UInt64 offset, MemoryProtection memoryProtection, uint size = 0, MemoryAllocationType allocationType = MemoryAllocationType.None) {
			return MapFileView(fileMapping, offset, IntPtr.Zero, memoryProtection, size, allocationType);
		}

		public IntPtr MapFileView(FileMapping fileMapping, UInt64 offset, IntPtr baseAddress, MemoryProtection memoryProtection, uint size = 0, MemoryAllocationType allocationType = MemoryAllocationType.None) {
			IntPtr result = MapViewOfFile2(fileMapping.handle, handle, offset, baseAddress, size, (uint)allocationType, (uint)memoryProtection);
			if(result == IntPtr.Zero) throw new Win32Exception();
			return result;
		}

		public void UnmapFileView(IntPtr baseAddress) {
			bool success = UnmapViewOfFile2(handle, baseAddress, 0);
			if(!success) throw new Win32Exception();
		}

		public string GetMappedFileName(IntPtr baseAddress) {
			var sb = new StringBuilder(300);
			var result = GetMappedFileNameW(handle, baseAddress, sb, (uint)sb.Capacity);
			if(result == 0) throw new Win32Exception();
			return sb.ToString();
		}

		public NativeToken OpenToken(TokenAccessLevels accessLevels) {
			bool success = OpenProcessToken(handle, (uint)accessLevels, out SafeTokenHandle tokenHandle);
			if(!success) throw new Win32Exception();
			return new NativeToken(tokenHandle);
		}

		public void SetValidCallTargets(IntPtr regionStartAddr, UInt32 RegionSize, ControlFlowCallTargetInfo[] OffsetInformation) {
			bool success = SetProcessValidCallTargets(handle, regionStartAddr, RegionSize, (uint) OffsetInformation.Length, OffsetInformation);
			if(!success) throw new Win32Exception();
		}

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Terminate(UInt32 exitCode) {
			bool success = TerminateProcess(handle, exitCode);
			if(!success) throw new Win32Exception();
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern SafeProcessHandle OpenProcess(UInt32 access, [MarshalAs(UnmanagedType.Bool)] bool inheritable, UInt32 processId);

		public static NativeProcess FromProcess(Process stdProcess) {
			return new NativeProcess(SafeProcessHandle.DuplicateFrom(stdProcess.Handle));
		}
		public static NativeProcess FromProcess(Process stdProcess, ProcessAccessRights accessRights) {
			return new NativeProcess(SafeProcessHandle.DuplicateFrom(stdProcess.Handle, (uint)accessRights));
		}

		public static implicit operator NativeProcess(Process stdProcess) => FromProcess(stdProcess);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern UInt32 GetProcessId(SafeProcessHandle handle);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetExitCodeProcess(SafeProcessHandle handle, out UInt32 exitCode);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool TerminateProcess(SafeProcessHandle handle, UInt32 exitCode);

		[DllImport("Ntdll.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern unsafe PInvoke.NTSTATUS NtQueryInformationProcess(SafeProcessHandle handle, ProcessInformationClass informationClass, void* buffer, uint bufferLength, out uint returnLength);

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

		[DllImport("User32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "IsImmersiveProcess")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool IsImmersiveProcessNative(SafeProcessHandle handle);

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

		[DllImport("Kernelbase.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "SetProcessValidCallTargets")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetProcessValidCallTargets(SafeProcessHandle handle, IntPtr regionStartAddr, UInt32 RegionSize, uint NumberOfOffsets, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] ControlFlowCallTargetInfo[] OffsetInformation);

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

		[DllImport("Psapi.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static unsafe extern bool GetProcessMemoryInfo(SafeProcessHandle handle, ref MemoryInfo.Native native, uint cb);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool QueryFullProcessImageName(SafeProcessHandle hProcess, Int32 flags, StringBuilder exename, ref UInt32 size);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsProcessCritical(SafeProcessHandle hProcess, [MarshalAs(UnmanagedType.Bool)] out bool Critical);

		[DllImport("kernel32.dll", SetLastError = true, EntryPoint = "FlushInstructionCache")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool FlushInstructionCacheNative(SafeProcessHandle hProcess, IntPtr baseAddr, UInt32 size);

		[DllImport("kernel32.dll", SetLastError = false)]
		static extern PInvoke.HResult EncodeRemotePointer(SafeProcessHandle hProcess, IntPtr plain, out IntPtr crypto);

		[DllImport("kernel32.dll", SetLastError = false)]
		static extern PInvoke.HResult DecodeRemotePointer(SafeProcessHandle hProcess, IntPtr crypto, out IntPtr plain);

		[DllImport("kernel32.dll", SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetProcessWorkingSetSizeEx(SafeProcessHandle hProcess, out UInt32 min, out UInt32 max, out UInt32 flags);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr MapViewOfFile2(SafeFileMappingHandle fileMapping, SafeProcessHandle processHandle, UInt64 Offset, IntPtr baseAddress, uint size, UInt32 allocationType, UInt32 pageProtection);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool UnmapViewOfFile2(SafeProcessHandle handle, IntPtr baseAddress, UInt32 flags);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern uint VirtualQueryEx(SafeProcessHandle handle, UIntPtr baseAddress, out MemoryBasicInformation.Native information, UInt32 dwLength);

		[DllImport("Psapi.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
		internal static unsafe extern int GetMappedFileNameW(SafeProcessHandle handle, IntPtr pv, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder filename, uint filenameBufferSize);

		[DllImport("Advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool OpenProcessToken(SafeProcessHandle procHandle, UInt32 access, out SafeTokenHandle tokenHandle);

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern unsafe bool CreateProcessW(
			[MarshalAs(UnmanagedType.LPWStr)] string lpApplicationName,
			[MarshalAs(UnmanagedType.LPWStr)] string lpCommandLine,
			SecurityAttributes * lpProcessAttributes,
			SecurityAttributes * lpThreadAttributes,
			[MarshalAs(UnmanagedType.Bool)] bool inheritHandles,
			UInt32 flags,
			void* environment,
			[MarshalAs(UnmanagedType.LPWStr)] string lpCurrentDirectory,
			StartupInfoW *startupInfo,
			ProcessInformation *processInfo
		);

	}
}
