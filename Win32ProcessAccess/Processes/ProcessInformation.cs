using System;

namespace Henke37.Win32.Processes {
	internal struct ProcessInformation {
		internal IntPtr hProcess;
		internal IntPtr hThread;
		internal UInt32 dwProcessId;
		internal UInt32 dwThreadId;
	}
}