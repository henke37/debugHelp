using Microsoft.Win32.SafeHandles;
using System;

namespace Henke37.Win32.Base.SafeHandles {
	internal class SafeFileObjectHandle : SafeKernelObjHandle, IEquatable<SafeFileObjectHandle>, IEquatable<SafeFileHandle> {
		internal static readonly SafeFileObjectHandle InvalidHandle=new SafeFileObjectHandle(InvalidHandleValue,false);

		internal SafeFileObjectHandle(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle) {
		}
		private SafeFileObjectHandle() : base(true) {
		}
		internal SafeFileObjectHandle(SafeFileHandle oldHandle) : base(true) {
			handle = DuplicateHandleLocal(oldHandle.DangerousGetHandle(), 0, false, DuplicateOptions.SameAccess);
		}

		public bool Equals(SafeFileObjectHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}
		public bool Equals(SafeFileHandle other) {
			return CompareObjectHandles(other.DangerousGetHandle(), handle);
		}

		public SafeFileHandle AsSafeFileHandle() {
			return new SafeFileHandle(DuplicateHandleLocal(handle, 0, false, DuplicateOptions.SameAccess), true);
		}


	}
}
