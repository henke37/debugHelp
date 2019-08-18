using System;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct IOCounters {
		UInt64 ReadOperationCount;
		UInt64 WriteOperationCount;
		UInt64 OtherOperationCount;
		UInt64 ReadTransferCount;
		UInt64 WriteTransferCount;
		UInt64 OtherTransferCount;
	}
}
