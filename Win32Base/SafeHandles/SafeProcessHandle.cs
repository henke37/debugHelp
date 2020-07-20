using System;
using System.Security.Permissions;

namespace Henke37.Win32.Base.SafeHandles {
	internal sealed class SafeProcessHandle : SafeKernelObjHandle, IEquatable<SafeProcessHandle> {

		public static SafeProcessHandle CurrentProcess {
#if NETFRAMEWORK
			[HostProtection(SelfAffectingProcessMgmt = true)]
#endif
			get {
				return new SafeProcessHandle((IntPtr)(-1), false);
			}
		}
		private SafeProcessHandle() : base(true) {
		}

		internal SafeProcessHandle(IntPtr handle, bool ownsHandle = true) : base(handle,ownsHandle) {
		}

		internal static SafeProcessHandle DuplicateFrom(IntPtr handle) {
			return new SafeProcessHandle(DuplicateHandleLocal(handle, 0, false, DuplicateOptions.SameAccess), true);
		}
		internal static SafeProcessHandle DuplicateFrom(IntPtr handle, uint accessRights) {
			return new SafeProcessHandle(DuplicateHandleLocal(handle, accessRights, false, DuplicateOptions.None), true);
		}

		public bool Equals(SafeProcessHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}

		public override bool IsInvalid {
			get {
				if(handle.ToInt32() == -1) return false;
				return base.IsInvalid;
			}
		}
	}
}
