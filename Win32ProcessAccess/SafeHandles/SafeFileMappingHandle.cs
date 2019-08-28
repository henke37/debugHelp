using System;

namespace Henke37.DebugHelp.Win32.SafeHandles {
	internal class SafeFileMappingHandle : SafeKernelObjHandle, IEquatable<SafeFileMappingHandle> {

		internal SafeFileMappingHandle(IntPtr handle, bool ownsHandle = true) : base(handle,ownsHandle) {
		}
		internal SafeFileMappingHandle() : base(true) {
		}

		public bool Equals(SafeFileMappingHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}
	}
}
