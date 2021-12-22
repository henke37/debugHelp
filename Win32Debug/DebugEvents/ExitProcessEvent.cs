namespace Henke37.Win32.Debug.Event {
	public class ExitProcessEvent : DebugEvent {
		public uint ExitCode;

		public ExitProcessEvent(uint processId, uint threadId, uint exitCode) : base(processId, threadId) {
			this.ExitCode = exitCode;
		}
	}
}