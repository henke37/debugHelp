using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace Henke37.DebugHelp.Win32 {
	internal abstract class SafeKernelObjHandle : SafeHandleZeroOrMinusOneIsInvalid {
		private const uint FlagInherit = 0x00000001;

		internal SafeKernelObjHandle(bool ownsHandle) : base(ownsHandle) {
		}

		internal void InitialSetHandle(IntPtr h) {
			Debug.Assert(base.IsInvalid, "Safe handle should only be set once");
			base.SetHandle(h);
		}

		[SuppressUnmanagedCodeSecurity]
		override protected bool ReleaseHandle() {
			return CloseHandle(handle);
		}

		public bool Inheritable {
			get {
				var success = GetHandleInformation(handle, out UInt32 flags);
				if(!success) throw new Win32Exception();
				return (flags & FlagInherit) != 0;
			}
			set {
				var success = SetHandleInformation(handle, FlagInherit, value ? FlagInherit : 0);
				if(!success) throw new Win32Exception();
			}
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CompareObjectHandles(IntPtr handle1, IntPtr handle2);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetHandleInformation(IntPtr handle, out UInt32 flags);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetHandleInformation(IntPtr handle, UInt32 mask, UInt32 flags);
	}
}
