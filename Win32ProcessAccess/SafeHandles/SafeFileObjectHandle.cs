using Microsoft.Win32.SafeHandles;
using System;

namespace Henke37.DebugHelp.Win32.SafeHandles {
	internal class SafeFileObjectHandle : SafeKernelObjHandle, IEquatable<SafeFileObjectHandle>, IEquatable<SafeFileHandle> {
		internal static readonly SafeFileObjectHandle InvalidHandle=new SafeFileObjectHandle(InvalidHandleValue,false);

		internal SafeFileObjectHandle(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle) {
		}
		private SafeFileObjectHandle() : base(true) {
		}

		public bool Equals(SafeFileObjectHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}
		public bool Equals(SafeFileHandle other) {
			return CompareObjectHandles(other.DangerousGetHandle(), handle);
		}

		public SafeFileHandle AsSafeFileHandle() {
			return new SafeFileHandle(NativeProcess.DuplicateHandleLocal(handle, 0, false, NativeProcess.DuplicateOptions.SameAccess), true);
		}


	}
}
