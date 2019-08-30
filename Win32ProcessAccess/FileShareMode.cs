using System;

namespace Henke37.DebugHelp.Win32 {
	[Flags]
	public enum FileShareMode : uint {
		None=0,
		Delete=4,
		Read=1,
		Write=2
	}
}