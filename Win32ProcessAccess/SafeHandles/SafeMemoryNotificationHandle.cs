using System;

namespace Henke37.DebugHelp.Win32.SafeHandles {
	internal class SafeMemoryNotificationHandle : SafeKernelObjHandle, IEquatable<SafeMemoryNotificationHandle> {
		public SafeMemoryNotificationHandle(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle) {
		}

		public bool Equals(SafeMemoryNotificationHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}
	}
}
