using System;

namespace Henke37.Win32.Debug.Event {
	public class OutputDebugStringEvent : DebugEvent {
		public IntPtr DataAddress;
		public bool IsUnicode;

		public OutputDebugStringEvent(uint processId, uint threadId, IntPtr dataAddress, bool isUnicode) : base(processId, threadId) {
			this.DataAddress = dataAddress;
			this.IsUnicode = isUnicode;
		}
	}
}