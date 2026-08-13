using System;
using System.Security;

namespace Henke37.Win32.SafeHandles {
	internal class SafeTokenHandle : SafeKernelObjHandle, IEquatable<SafeTokenHandle> {
		public SafeTokenHandle(IntPtr newHandle, bool ownsHandle=true) : base(newHandle, ownsHandle) {
		}

		private SafeTokenHandle() : base(true) {
		}

		[SecuritySafeCritical]
		public bool Equals(SafeTokenHandle other) {
			if(other is null) return false;
			if(other.handle == handle) return true;
			return CompareObjectHandles(handle, other.handle);
		}
	}
}
