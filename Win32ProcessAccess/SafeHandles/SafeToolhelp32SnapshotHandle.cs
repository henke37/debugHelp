using System;
using Henke37.Win32.SafeHandles;

namespace Henke37.Win32.SafeHandles {
	internal class SafeToolhelp32SnapshotHandle : SafeKernelObjHandle, IEquatable<SafeToolhelp32SnapshotHandle> {
		internal SafeToolhelp32SnapshotHandle(IntPtr handle) : base(handle, true) {
		}
		private SafeToolhelp32SnapshotHandle() : base(true) {
		}

		public bool Equals(SafeToolhelp32SnapshotHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}
	}
}