using System;

namespace Henke37.DebugHelp.Win32.SafeHandles {
	internal class SafeMemoryResourceNotificationHandle : SafeKernelObjHandle, IEquatable<SafeMemoryResourceNotificationHandle> {
		internal SafeMemoryResourceNotificationHandle(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle) {
		}
		internal SafeMemoryResourceNotificationHandle() : base(true) {
		}

		public bool Equals(SafeMemoryResourceNotificationHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}
	}
}
