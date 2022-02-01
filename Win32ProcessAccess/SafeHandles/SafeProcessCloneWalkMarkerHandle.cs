using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Henke37.Win32.SafeHandles {
	class SafeProcessCloneWalkMarkerHandle : SafeHandleZeroOrMinusOneIsInvalid {

		public SafeProcessCloneWalkMarkerHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle) {
			InitialSetHandle(handle);
		}

		public SafeProcessCloneWalkMarkerHandle(bool ownsHandle) : base(ownsHandle) {
		}

		internal void InitialSetHandle(IntPtr h) {
			Debug.Assert(base.IsInvalid, "Safe handle should only be set once");
			base.SetHandle(h);
		}

		protected override bool ReleaseHandle() {
			var ret = PssWalkMarkerFree(handle);
			return ret == 0;
		}


		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		public static extern Int32 PssWalkMarkerFree(IntPtr handle);
	}
}
