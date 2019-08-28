using System;

namespace Henke37.DebugHelp.Win32.SafeHandles {
	internal class SafeFileMappingHandle : SafeKernelObjHandle, IEquatable<SafeFileMappingHandle> {

		public SafeFileMappingHandle(IntPtr handle, bool ownsHandle = true) : base(handle,ownsHandle) {
		}

		public bool Equals(SafeFileMappingHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}
	}
}
