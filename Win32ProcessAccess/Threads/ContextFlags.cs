using System;

namespace Henke37.Win32.Threads {
	[Flags]
	internal enum ContextFlags : UInt32 {
		i386  = 0x00010000,
		ARM   = 0x00200000,
		ARM64 = 0x00400000,
		AMD64 = 0x00100000,

		Control       = 0x00000001,
		Integer       = 0x00000002,
		Segments      = 0x00000004,
		FloatingPoint = 0x00000008,
		Debug         = 0x00000010,
		Extended      = 0x00000020
	}
}