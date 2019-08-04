using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Henke37.DebugHelp.Win32 {
	public sealed class NativeThread : IDisposable {
		readonly SafeThreadHandle handle;

		public NativeThread(UInt32 threadId) : this(ThreadAcccessRights.All, threadId) {
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public NativeThread(ThreadAcccessRights access, UInt32 threadId) {
			handle = OpenThread((UInt32)access, true, threadId);
			if(handle.IsInvalid) throw new Win32Exception();
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

		public ThreadContext GetContext() {
			var ctx = new ThreadContext32();
			ctx.ReadFromHandle(handle);
			return ctx;
		}

		public void SetContext(ThreadContext32 context) {
			context.WriteToHandle(handle);
		}


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

		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static extern SafeThreadHandle OpenThread(UInt32 access, bool inheritable, UInt32 threadId);

		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static extern UInt32 GetThreadId(SafeThreadHandle handle);
		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static extern UInt32 GetProcessIdOfThread(SafeThreadHandle handle);
		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static extern unsafe bool GetExitCodeThread(SafeThreadHandle handle, UInt32 *exitCode);

		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static extern Int32 SuspendThread(SafeThreadHandle handle);
		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static extern Int32 ResumeThread(SafeThreadHandle handle);

		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static extern bool TerminateThread(SafeThreadHandle handle, UInt32 exitCode);
	}
}
