using System;

namespace Henke37.Win32.Debug.Event {
	public class UnloadDllEvent : DebugEvent {
		public IntPtr LoadBase;

		public UnloadDllEvent(uint processId, uint threadId, IntPtr loadBase) : base(processId, threadId) {
			this.LoadBase = loadBase;
		}
	}
}