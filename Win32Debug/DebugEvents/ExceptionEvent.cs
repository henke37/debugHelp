namespace Henke37.Win32.Debug.Event {
	internal class ExceptionEvent : DebugEvent {
		public ExceptionRecord ExceptionRecord;
		public bool FirstChance;

		public ExceptionEvent(uint processId, uint threadId, ExceptionRecord exceptionRecord, bool firstChance) : base(processId, threadId) {
			this.ExceptionRecord = exceptionRecord;
			this.FirstChance = firstChance;
		}
	}
}