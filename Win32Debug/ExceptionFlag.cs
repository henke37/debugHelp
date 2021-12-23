using System;

namespace Henke37.Win32.Debug {
	[Flags]
	public enum ExceptionFlag {
		Noncontinuable,
		SoftwareOriginate
	}
}