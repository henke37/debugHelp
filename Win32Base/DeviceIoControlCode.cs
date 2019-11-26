namespace Henke37.Win32.Base {
	internal enum DeviceIoControlCode : uint {
		StorageEjectMedia = 0x6D8001,
		StorageLoadMedia = 0x6DC001,
		DiskEjectMedia = 0x74808,
		DiskLoadMedia = 0x7480c,
		DvdGetRegion= 0x335014,
		CdRomReadTOC= 0x24000,
		CdRomReadQChannel = 0x2402c,
		CdRomLoadMedia= 0x2480c,
		CdRomEjectMedia= 0x24808,
		BatteryQueryTag= 0x294040,
		BatteryQueryInformation= 0x294044,
		BatteryQueryStatus= 0x29404c,
		BatterySetInformation = 0x298048
	}
}