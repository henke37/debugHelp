using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace Henke37.DebugHelp.Win32 {
	internal sealed class SafeThreadHandle : SafeHandleZeroOrMinusOneIsInvalid, IEquatable<SafeThreadHandle> {
		internal SafeThreadHandle() : base(true) {
		}

		internal void InitialSetHandle(IntPtr h) {
			Debug.Assert(base.IsInvalid, "Safe handle should only be set once");
			base.SetHandle(h);
		}

		[SuppressUnmanagedCodeSecurity]
		override protected bool ReleaseHandle() {
			return CloseHandle(handle);
		}

		public bool Equals(SafeThreadHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CompareObjectHandles(IntPtr handle1, IntPtr handle2);
	}
}
