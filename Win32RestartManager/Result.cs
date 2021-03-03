using System;

namespace Henke37.Win32.Restart {
	enum Result : UInt32 {
		Success = 0,
		InvalidHandle = 6,
		OutOfMemory = 14,
		WriteFault = 29,
		SemTimeout = 121,
		BadArguments = 160,
		FileNotFound,
		MoreData = 234,
		Canceled = 1223
	}
}
