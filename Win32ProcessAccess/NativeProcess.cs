using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
	public class NativeProcess : IDisposable {
		internal SafeProcessHandle handle;

		internal NativeProcess(SafeProcessHandle handle) {
			if(handle.IsInvalid) throw new ArgumentException("Handle must be valid!", nameof(handle));
			this.handle = handle;
		}

		public static NativeProcess Open(int processId, ProcessAccessRights rights = ProcessAccessRights.All) {
			var proc = Open(processId, out var firstThread, rights);
			firstThread.Close();
			return proc;
		}
		public static NativeProcess Open(int processId, out NativeThread firstThread, ProcessAccessRights rights = ProcessAccessRights.All) {
			throw new NotImplementedException();
		}

		public void Dispose() => handle.Dispose();
		public void Close() => handle.Close();

		public UInt32 ProcessId => GetProcessId(handle);
		public UInt32 ExitCode {
			get {
				var success=GetExitCodeProcess(handle, out uint exitCode);
				if(!success) throw new Win32Exception();
				return exitCode;
			}
		}

		public void Terminate(UInt32 exitCode) {
			bool success=TerminateProcess(handle, exitCode);
			if(!success) throw new Win32Exception();
		}

		public static NativeProcess FromProcess(Process stdProcess) {
			return new NativeProcess(new SafeProcessHandle(DuplicateHandleLocal(stdProcess.Handle,0,false, DuplicateOptions.SameAccess), true));
		}
		public static NativeProcess FromProcess(Process stdProcess, ProcessAccessRights accessRights) {
			return new NativeProcess(new SafeProcessHandle(DuplicateHandleLocal(stdProcess.Handle, (uint)accessRights, false, DuplicateOptions.None), true));
		}

		public static implicit operator NativeProcess(Process stdProcess) => FromProcess(stdProcess);

		internal static unsafe IntPtr DuplicateHandleLocal(IntPtr sourceHandle, uint desiredAccess, bool inheritHandle, DuplicateOptions options) {
			IntPtr newHandle=IntPtr.Zero;
			bool success = DuplicateHandle(SafeProcessHandle.CurrentProcess, sourceHandle, SafeProcessHandle.CurrentProcess, (IntPtr)(int)&newHandle, desiredAccess, inheritHandle, options);

			if(!success) throw new Win32Exception();
			return newHandle;
		}

		[Flags]
		public enum DuplicateOptions : uint {
			None=0,
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
	}
}
