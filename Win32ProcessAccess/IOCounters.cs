using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32 {

	[StructLayout(LayoutKind.Sequential)]
	public struct IOCounters {
		UInt64 ReadOperationCount;
		UInt64 WriteOperationCount;
		UInt64 OtherOperationCount;
		UInt64 ReadTransferCount;
		UInt64 WriteTransferCount;
		UInt64 OtherTransferCount;

		internal IOCounters(int dummy) : this() {
			if(dummy != 0) throw new ArgumentException();
			ReadOperationCount = 0;
			WriteOperationCount = 0;
			OtherOperationCount = 0;
			ReadTransferCount = 0;
			WriteTransferCount = 0;
			OtherTransferCount = 0;
		}
	}
}
