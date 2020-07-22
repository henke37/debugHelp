using System;

namespace Henke37.Win32.AccessRights {
	[Flags]
	public enum FileMappingAccessRights : uint {
		None=0,
		Execute = 0x0020,
		Read=4,
		Write=2,
		All=Read|Write|Execute
	}
}
