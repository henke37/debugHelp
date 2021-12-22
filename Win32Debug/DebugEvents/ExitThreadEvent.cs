namespace Henke37.Win32.Debug.Event {
	public class ExitThreadEvent : DebugEvent {
		public uint ExitCode;

		public ExitThreadEvent(uint processId, uint threadId, uint exitCode) : base(processId, threadId) {
			this.ExitCode = exitCode;
		}
	}
}