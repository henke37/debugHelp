using Henke37.Win32.SafeHandles;
using Microsoft.Win32.SafeHandles;
using System;

namespace Henke37.Win32.Debug.Event {
	internal class LoadDllEvent : DebugEvent {
		private SafeFileObjectHandle safeFileObjectHandle;
		public IntPtr ImageBase;

		public SafeFileHandle FileHandle => safeFileObjectHandle.AsSafeFileHandle();

		public LoadDllEvent(uint processId, uint threadId, SafeFileObjectHandle safeFileObjectHandle, IntPtr imageBase) : base(processId, threadId) {
			this.safeFileObjectHandle = safeFileObjectHandle;
			this.ImageBase = imageBase;
		}
	}
}