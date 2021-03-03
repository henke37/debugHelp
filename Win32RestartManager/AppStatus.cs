using System;

namespace Henke37.Win32.Restart {
	[Flags]
	internal enum AppStatus : UInt32 {
		Unknown = 0,
		Running = 0x1,
		Stopped = 0x2,
		StoppedOther = 0x4,
		Restarted = 0x8,
		ErrorOnStop = 0x10,
		ErrorOnRestart = 0x20,
		ShutdownMasked = 0x40,
		RestartMasked = 0x80
	}
}