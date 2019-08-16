using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Henke37.DebugHelp.Win32 {
	public sealed class NativeThread : IDisposable {
		readonly SafeThreadHandle handle;

		public static NativeThread Open(UInt32 threadId) => Open(threadId, ThreadAcccessRights.All);

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static NativeThread Open(UInt32 threadId, ThreadAcccessRights access) {
			SafeThreadHandle handle = OpenThread((UInt32)access, true, threadId);
			if(handle.IsInvalid) throw new Win32Exception();
			return new NativeThread(handle);
		}

		internal NativeThread(SafeThreadHandle handle) {
			this.handle = handle;
		}

		public void Dispose() => handle.Dispose();
		public void Close() => handle.Close();

		public UInt32 ThreadId {

			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				UInt32 threadId = GetThreadId(handle);
				if(threadId == 0) throw new Win32Exception();
				return threadId;
			}
		}

		public UInt32 ProcessId {

			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				UInt32 processId = GetProcessIdOfThread(handle);
				if(processId == 0) throw new Win32Exception();
				return processId;
			}
		}

		public unsafe UInt32 ExitCode {

			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				UInt32 exitCode;
				bool success=GetExitCodeThread(handle,&exitCode);
				if(!success) throw new Win32Exception();

				return exitCode;
			}
		}

		public unsafe string Description {

			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				char* descPtr;				
				UInt32 status=GetThreadDescription(handle, &descPtr);
				if((status & 0x80000000) == 0x80000000) throw new Exception();
				string desc = new string(descPtr);
				Marshal.FreeHGlobal((IntPtr)descPtr);

				return desc;
			}
		}

#if x86
		public ThreadContext32 GetContext() {
			var ctx = new ThreadContext32();
			ctx.ReadFromHandle(handle);
			return ctx;
		}
		public void SetContext(ThreadContext32 context) {
			context.WriteToHandle(handle);
		}
#elif x64
		public ThreadContext64 GetContext() {
			var ctx = new ThreadContext64();
			ctx.ReadFromHandle(handle);
			return ctx;
		}
		public void SetContext(ThreadContext64 context) {
			context.WriteToHandle(handle);
		}

		public ThreadContext32 GetWow64Context() {
			var ctx = new ThreadContext32();
			ctx.ReadFromHandle(handle);
			return ctx;
		}
		public void SetWow64Context(ThreadContext32 context) {
			context.WriteToHandle(handle);
		}
#else
#error "No GetContext implementation"
#endif



		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Suspend() {
			SuspendThread(handle);
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Resume() {
			ResumeThread(handle);
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Terminate(UInt32 exitCode) {
			TerminateThread(handle, exitCode);
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern SafeThreadHandle OpenThread(UInt32 access, [MarshalAs(UnmanagedType.Bool)] bool inheritable, UInt32 threadId);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern UInt32 GetThreadId(SafeThreadHandle handle);
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern UInt32 GetProcessIdOfThread(SafeThreadHandle handle);
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool GetExitCodeThread(SafeThreadHandle handle, UInt32 *exitCode);


		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = false)]
		internal static extern unsafe UInt32 GetThreadDescription(SafeThreadHandle handle, Char **exitCode);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern Int32 SuspendThread(SafeThreadHandle handle);
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern Int32 ResumeThread(SafeThreadHandle handle);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool TerminateThread(SafeThreadHandle handle, UInt32 exitCode);
	}
}
