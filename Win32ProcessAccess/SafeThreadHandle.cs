using System;

namespace Henke37.DebugHelp.Win32 {
	internal sealed class SafeThreadHandle : SafeKernelObjHandle, IEquatable<SafeThreadHandle> {
		internal SafeThreadHandle() : base(true) {
		}

		public bool Equals(SafeThreadHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}
	}
}
