using System;

namespace Henke37.DebugHelp.Win32 {
	[Flags]
	public enum EFlags {
		None = 0,
		Carry = 1 << 0,
		Parity = 1 << 2,
		Aux = 1 << 4,
		Zero = 1 << 6,
		Sign = 1 << 7,
		Trap = 1 << 8,
		If = 1 << 9,
		Dir = 1 << 10,
		Overflow = 1 << 11,
		Resume = 1 << 16,
		Virtual = 1 << 17,
		Align = 1 << 18,
		VirtualInterupt = 1 << 19,
		VirtualPending = 1 << 20,
		CPUId = 1 << 21
	}
}
