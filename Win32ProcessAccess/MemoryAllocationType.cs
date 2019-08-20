using System;

namespace Henke37.DebugHelp.Win32 {
	[Flags]
	public enum MemoryAllocationType : uint {
		Commit = 0x00001000,
		Reserve = 0x00002000,
		Reset = 0x00080000,
		ResetUndo = 0x1000000,
		LargePages = 0x20000000,
		Physical = 0x00400000,
		TopDown = 0x00100000
	}
}