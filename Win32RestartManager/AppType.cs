using System;

namespace Henke37.Win32.Restart {
	internal enum AppType : UInt32 {
		UnknownApp = 0,
		MainWindow,
		OtherWindow,
		Service,
		Explorer,
		Console,
		Critical = 1000
	}
}