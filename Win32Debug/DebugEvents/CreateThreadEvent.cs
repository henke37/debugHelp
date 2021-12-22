using Henke37.Win32.SafeHandles;
using System;

namespace Henke37.Win32.Debug.Event {
	internal class CreateThreadEvent : DebugEvent {
		private SafeThreadHandle safeThreadHandle;
		private IntPtr startAddress;
		private IntPtr localBase;

		public CreateThreadEvent(uint processId, uint threadId, SafeThreadHandle safeThreadHandle, IntPtr startAddress, IntPtr localBase) : base(processId, threadId) {
			this.safeThreadHandle = safeThreadHandle;
			this.startAddress = startAddress;
			this.localBase = localBase;
		}
	}
}