using System;

namespace Henke37.DebugHelp.Win32.SafeHandles {
	internal sealed class SafeThreadHandle : SafeKernelObjHandle, IEquatable<SafeThreadHandle> {
		internal SafeThreadHandle(IntPtr handle) : base(handle, true) {
		}
		private SafeThreadHandle() : base(true) {
		}

		public bool Equals(SafeThreadHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}
	}
}
