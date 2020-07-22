using System;

namespace Henke37.Win32.Files {
	[Flags]
	public enum FileShareMode : uint {
		None=0,
		Delete=4,
		Read=1,
		Write=2
	}
}