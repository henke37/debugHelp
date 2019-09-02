namespace Henke37.Win32.Base {
	internal enum DeviceIoControlCode : uint {
		StorageEjectMedia = 0x6D8001,
		StorageLoadMedia = 0x6DC001,
		DiskEjectMedia = 0x74808,
		DiskLoadMedia = 0x7480c,
		DvdGetRegion= 0x335014
	}
}