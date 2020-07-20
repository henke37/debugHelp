using System;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32.Jobs {
	[StructLayout(LayoutKind.Sequential)]
	internal struct EndOfJobTimeActionStruct {
		public EndOfJobTimeAction endOfJobTimeAction;

		public EndOfJobTimeActionStruct(EndOfJobTimeAction value) : this() {
			this.endOfJobTimeAction = value;
		}
	}

	public enum EndOfJobTimeAction {
		Terminate = 0,
		Post = 1
	}
}
