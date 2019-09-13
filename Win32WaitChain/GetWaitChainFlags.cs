using System;

namespace Henke37.Win32.WaitChain {
	[Flags]
	public enum GetWaitChainFlags : uint {
		OutOfProccess=1,
		OutOfProcessCOM=2,
		OutOfProcessCriticalSection=4
	}
}
