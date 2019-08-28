using System;

namespace Henke37.DebugHelp.Win32 {
	public enum MemoryProtection : uint {
		None = 0,
		Execute = 0x10,
		ExecuteRead = 0x20,
		ExecuteReadWrite = 0x40,
		ExecuteWriteCopy = 0x80,
		NoAccess = 0x01,
		ReadOnly = 0x02,
		ReadWrite = 0x04,
		WriteCopy = 0x08,
		Guard = 0x100,
		NoCache = 0x200,
		WriteCombine = 0x400
	}

	public enum MemoryBackingType {
		Image = 0x1000000,
		Mapped = 0x40000,
		Private = 0x20000
	}
}