using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace Henke37.Win32.SafeHandles {
	internal class SafeProcessCloneHandle : SafeHandleZeroOrMinusOneIsInvalid {
		public SafeProcessCloneHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle) {
			InitialSetHandle(handle);
		}

		public SafeProcessCloneHandle(bool ownsHandle) : base(ownsHandle) {
		}

		internal void InitialSetHandle(IntPtr h) {
			Debug.Assert(base.IsInvalid, "Safe handle should only be set once");
			base.SetHandle(h);
		}

		[SecuritySafeCritical]
		override protected bool ReleaseHandle() {
			var ret=PssFreeSnapshot(SafeProcessHandle.CurrentProcess,handle);

			return ret == 0;
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		[SuppressUnmanagedCodeSecurity]
		public static extern Int32 PssFreeSnapshot(SafeProcessHandle ownerProcess, IntPtr handle);
	}
}
