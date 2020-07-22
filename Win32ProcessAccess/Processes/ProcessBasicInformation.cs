using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Processes {
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct ProcessBasicInformation {
		IntPtr Reserved1;
		internal IntPtr PebBaseAddress;
		IntPtr Reserved2_1;
		IntPtr Reserved2_2;
		internal UInt32 UniqueProcessId;
		IntPtr Reserved3;
	}
}
