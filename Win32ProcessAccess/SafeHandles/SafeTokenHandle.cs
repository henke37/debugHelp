using System;
using Henke37.Win32.Base.SafeHandles;
namespace Henke37.DebugHelp.Win32.SafeHandles {
	internal class SafeTokenHandle : SafeKernelObjHandle, IEquatable<SafeTokenHandle> {
		private SafeTokenHandle() : base(true) {
		}

		public bool Equals(SafeTokenHandle other) {
			return CompareObjectHandles(handle, other.handle);
		}
	}
}
