using System;

namespace Henke37.Win32.Restart {
	enum RMResult : UInt32 {
		Success = 0,
		SemTimeout = 121,
		WriteFault = 29,
		OutOfMemory = 14,
		InvalidHandle = 6,
		BadArguments = 160,
		FileNotFound
	}
}
