using System;

namespace Henke37.Win32.Clone {
	internal enum WalkInformationClass : UInt32 {
		AUXILIARY_PAGES = 0,
		VA_SPACE = 1,
		HANDLES = 2,
		THREADS = 3
	}
}
