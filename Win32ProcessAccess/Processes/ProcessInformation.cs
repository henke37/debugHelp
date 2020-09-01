using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Processes {
	[StructLayout(LayoutKind.Sequential)]
	internal struct ProcessInformation {
		internal IntPtr hProcess;
		internal IntPtr hThread;
		internal UInt32 dwProcessId;
		internal UInt32 dwThreadId;
	}
}