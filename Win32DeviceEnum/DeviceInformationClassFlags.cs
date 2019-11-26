using System;

namespace Henke37.Win32.DeviceEnum {
	[Flags]
	internal enum DeviceInformationClassFlags : UInt32 {
		None=0,
		Default = 0x00000001,
		Present = 0x00000002,
		AllClasses = 0x00000004,
		Profile = 0x00000008,
		DeviceInterface = 0x00000010
	}
}
