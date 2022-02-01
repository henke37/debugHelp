using Henke37.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Clone {
	class Walker : IDisposable {
		private ProcessClone clone;
		private SafeProcessCloneWalkMarkerHandle markerHandle;

		public Walker(ProcessClone processClone) {
			clone = processClone;
			var ret=PssWalkMarkerCreate(IntPtr.Zero, processClone.Handle, out markerHandle);
			if(ret != 0) throw new Win32Exception(ret);
		}

		public bool MoveNext() {
			throw new NotImplementedException();
		}

		public void Reset() {
			PssWalkMarkerSeekToBeginning(markerHandle);
		}

		public void Dispose() {
			markerHandle.Dispose();
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		public static extern Int32 PssWalkMarkerCreate(IntPtr allocator, SafeProcessCloneHandle cloneHandle, out SafeProcessCloneWalkMarkerHandle walkerHandle);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		public static extern Int32 PssWalkMarkerSetPosition(SafeProcessCloneWalkMarkerHandle walkerHandle, IntPtr position);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		public static extern Int32 PssWalkMarkerGetPosition(SafeProcessCloneWalkMarkerHandle walkerHandle, out IntPtr position);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		public static extern Int32 PssWalkMarkerSeekToBeginning(SafeProcessCloneWalkMarkerHandle walkerHandle);
	}
}
