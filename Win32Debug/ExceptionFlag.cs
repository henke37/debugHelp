using System;

namespace Henke37.Win32.Debug {
	[Flags]
	public enum ExceptionFlag {
		Noncontinuable=1,
		Unwinding = 2,
		ExitUnwind = 4,
		StackInvalid = 8,
		NestedCall = 0x10,
		TargetUnwind = 0x20,
		CollidedUnwind = 0x40,
		UnwindMask = 0x66
	}
}