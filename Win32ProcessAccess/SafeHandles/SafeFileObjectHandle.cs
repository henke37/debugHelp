using System;

namespace Henke37.DebugHelp.Win32.SafeHandles {
	internal class SafeFileObjectHandle : SafeKernelObjHandle, IEquatable<SafeFileObjectHandle> {
		internal static readonly SafeFileObjectHandle InvalidHandle=new SafeFileObjectHandle(InvalidHandleValue,false);

		internal SafeFileObjectHandle(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle) {
		}
		internal SafeFileObjectHandle() : base(true) {
		}

		public bool Equals(SafeFileObjectHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}
	}
}
