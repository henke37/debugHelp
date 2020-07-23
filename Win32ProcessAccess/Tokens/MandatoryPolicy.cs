using System;

namespace Henke37.Win32.Tokens {
	[Flags]
	public enum MandatoryPolicy {
		Off = 0,
		NoWriteUp = 1,
		NewProcessMin = 2
	}
}
