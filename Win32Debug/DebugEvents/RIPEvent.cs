namespace Henke37.Win32.Debug.Event {
	public class RIPEvent : DebugEvent {
		public uint Error;
		public uint Type;

		public RIPEvent(uint processId, uint threadId, uint error, uint type) : base(processId, threadId) {
			this.Error = error;
			this.Type = type;
		}
	}
}