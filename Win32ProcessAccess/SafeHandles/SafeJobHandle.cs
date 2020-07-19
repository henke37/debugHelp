using Henke37.Win32.Base.SafeHandles;
using System;

namespace Henke37.DebugHelp.Win32.SafeHandles {
	class SafeJobHandle : SafeKernelObjHandle, IEquatable<SafeJobHandle> {
		private SafeJobHandle() : base(true) {
		}

		internal SafeJobHandle(IntPtr handle, bool ownsHandle = true) : base(handle, ownsHandle) {
		}

		internal static SafeJobHandle DuplicateFrom(IntPtr handle) {
			return new SafeJobHandle(DuplicateHandleLocal(handle, 0, false, DuplicateOptions.SameAccess), true);
		}
		internal static SafeJobHandle DuplicateFrom(IntPtr handle, uint accessRights) {
			return new SafeJobHandle(DuplicateHandleLocal(handle, accessRights, false, DuplicateOptions.None), true);
		}

		public bool Equals(SafeJobHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}
	}
}
