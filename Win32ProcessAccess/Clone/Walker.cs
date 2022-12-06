using Henke37.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

using static Henke37.Win32.Clone.WalkerNativeMethods;

namespace Henke37.Win32.Clone {
	class Walker<T> : IDisposable, IEnumerator<T> where T : unmanaged {
		private ProcessClone clone;
		private SafeProcessCloneWalkMarkerHandle markerHandle;
		private WalkInformationClass informationClass;

		private const Int32 ERROR_NO_MORE_ITEMS = 259;

		public T Current => _current;
		object IEnumerator.Current => _current;
		private T _current;

		public Walker(ProcessClone processClone, WalkInformationClass informationClass) {
			clone = processClone;
			this.informationClass = informationClass;
			markerHandle=CreateMarker(processClone);
		}

		private SafeProcessCloneWalkMarkerHandle CreateMarker(ProcessClone processClone) {
			var ret = PssWalkMarkerCreate(IntPtr.Zero, processClone.Handle, out var markerHandle);
			if(ret != 0) throw new Win32Exception(ret);
			return markerHandle;
		}

		[SecuritySafeCritical]
		private unsafe bool Walk(ref T buff) {
			fixed(void* buffP = &buff) {
				var ret = PssWalkSnapshot(clone.Handle, informationClass, markerHandle, buffP, (uint)sizeof(T));
				if(ret == ERROR_NO_MORE_ITEMS) return false;
				if(ret != 0) throw new Win32Exception(ret);
				return true;
			}
		}

		public void Reset() {
			PssWalkMarkerSeekToBeginning(markerHandle);
		}

		public void Dispose() {
			markerHandle.Dispose();
		}

		public bool MoveNext() {
			return Walk(ref _current);
		}
	}

	[SuppressUnmanagedCodeSecurity]
	static class WalkerNativeMethods {
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern Int32 PssWalkMarkerCreate(IntPtr allocator, SafeProcessCloneHandle cloneHandle, out SafeProcessCloneWalkMarkerHandle walkerHandle);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern Int32 PssWalkMarkerSetPosition(SafeProcessCloneWalkMarkerHandle walkerHandle, IntPtr position);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern Int32 PssWalkMarkerGetPosition(SafeProcessCloneWalkMarkerHandle walkerHandle, out IntPtr position);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern Int32 PssWalkMarkerSeekToBeginning(SafeProcessCloneWalkMarkerHandle walkerHandle);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		internal static unsafe extern Int32 PssWalkSnapshot(SafeProcessCloneHandle clonedProc, WalkInformationClass informationClass, SafeProcessCloneWalkMarkerHandle markerHandle, void* buffer, UInt32 buffSize);
	}
}
