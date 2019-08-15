using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
	internal sealed class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid {
		internal SafeProcessHandle() : base(true) {
		}

		public SafeProcessHandle(IntPtr handle, bool ownsHandle = true) : base(ownsHandle) {
			this.handle = handle;
		}

		internal void InitialSetHandle(IntPtr h) {
			Debug.Assert(base.IsInvalid, "Safe handle should only be set once");
			base.SetHandle(h);
		}

		override protected bool ReleaseHandle() {
			return CloseHandle(handle);
		}

		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		public static extern bool CloseHandle(IntPtr handle);
	}
}
