using System;
using System.Security.Permissions;

namespace Henke37.Win32.SafeHandles {
	internal sealed class SafeThreadHandle : SafeKernelObjHandle, IEquatable<SafeThreadHandle> {
		internal SafeThreadHandle(IntPtr handle, bool ownsHandle = true) : base(handle, ownsHandle) {
		}
		internal SafeThreadHandle(IntPtr handle) : base(handle, true) {
		}
		private SafeThreadHandle() : base(true) {
		}

		public static SafeThreadHandle CurrentThread {
#if NETFRAMEWORK
			[HostProtection(SelfAffectingThreading = true)]
#endif
			get {
				return new SafeThreadHandle((IntPtr)(-2), false);
			}
		}

		public bool Equals(SafeThreadHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}

		public override bool IsInvalid {
			get {
				if(handle.ToInt32() == -2) return false;
				return base.IsInvalid;
			}
		}
	}
}
