﻿namespace Henke37.Win32.Base {
	internal enum DeviceIoControlCode : uint {
		StorageEjectMedia = 0x6D8001,
		StorageLoadMedia = 0x6DC001,
		StorageGetDeviceNumber = 0x2d1080,
		DiskEjectMedia = 0x74808,
		DiskLoadMedia = 0x7480c,
		DvdGetRegion= 0x335014,
		CdRomReadTOC= 0x24000,
		CdRomReadTOCEx= 0x24054,
		CdRomReadQChannel = 0x2402c,
		CdRomLoadMedia= 0x2480c,
		CdRomEjectMedia= 0x24808,
		CdRomRawRead = 0x2403e,
		CdRomExclusiveAccess = 0x2c05c,
		CdRomGetConfiguration = 0x24058,
		CdRomGetControl = 0x24034,
		CdRomGetDriveGeometry = 0x2404c,
		CdRomGetDriveGeometryEx = 0x24050,
		CdRomGetLastSession= 0x24038,
		CdRomGetPerformance = 0x24070,
		CdRomPlayAudio = 0x24018,
		CdRomPauseAudio = 0x2400c,
		CdRomResumeAudio = 0x24010,
		CdRomSeekAudio = 0x24004,
		CdRomStopAudio = 0x24008,
		BatteryQueryTag = 0x294040,
		BatteryQueryInformation= 0x294044,
		BatteryQueryStatus= 0x29404c,
		BatterySetInformation = 0x298048,
		IdePassThrough = 0x4d028
	}
}