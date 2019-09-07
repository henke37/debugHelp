using System;
using Henke37.Win32.Base.SafeHandles;
namespace Henke37.DebugHelp.Win32.SafeHandles {
	internal class SafeTokenHandle : SafeKernelObjHandle, IEquatable<SafeTokenHandle> {
		public SafeTokenHandle(IntPtr newHandle, bool ownsHandle=true) : base(newHandle, ownsHandle) {
		}

		private SafeTokenHandle() : base(true) {
		}

		public bool Equals(SafeTokenHandle other) {
			return CompareObjectHandles(handle, other.handle);
		}
	}
}
