using System.Collections.Generic;

namespace Henke37.DebugHelp.Gdb {

	public abstract class StopPacket {
	}

	public class StopReason : StopPacket {
		public string Reason;
		public object ReasonArg;
		public int ProcessId;
		public int ThreadId;
		public int Core;
		public int Signal;

		public Dictionary<int, byte[]> Registers;
	}
	public abstract class ProcessEndPacket : StopPacket {
		public int ProccessId;

		protected ProcessEndPacket() {
		}
		protected ProcessEndPacket(int pid) {
			ProccessId = pid;
		}
	}
	public class ProcessExit : ProcessEndPacket {
		public int ExitCode;

		public ProcessExit(int v1, int v2) {
		}
	}
	public class ProcessTermination : ProcessEndPacket {
		public int Signal;

		public ProcessTermination(int sig) {
			this.Signal = sig;
		}

		public ProcessTermination(int sig, int pid) : base(pid) {
			this.Signal = sig;
		}
	}
	public class ThreadExit : StopPacket {
		public int ExitCode;
		public int ThreadId;
	}
	public class NoResumedThreads : StopPacket { }
	public class ConsoleOutput : StopPacket {
		public string Text;
	}
}