using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.DebugHelp.Win32.Jobs {
	[StructLayout(LayoutKind.Sequential)]
	internal struct IOCounters {
		UInt64 ReadOperationCount;
		UInt64 WriteOperationCount;
		UInt64 OtherOperationCount;
		UInt64 ReadTransferCount;
		UInt64 WriteTransferCount;
		UInt64 OtherTransferCount;

		public IOCounters(int dummy) : this() {
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
