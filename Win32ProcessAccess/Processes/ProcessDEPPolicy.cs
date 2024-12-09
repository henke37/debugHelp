using System;

namespace Henke37.Win32.Processes {
	[Flags]
	public enum ProcessDEPPolicy : uint {
		None = 0,
		Enabled = 1,
		ATLThunkEmulation= 2,
		Permanent = 0x80000000
	}
}