using System;

namespace Henke37.Win32.Windows {
	public enum DisplayAffinity : UInt32 {
		None = 0,
		Monitor = 0x00000001,
		ExcludeFromCapture = 0x00000011
	}
}