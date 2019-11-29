using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.CdAccess {

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct StorageDeviceNumber {
		internal DeviceType deviceType;
		internal UInt32 DeviceNumber;
		internal UInt32 PartionNumber;

		public string GetDeviceName() {
			switch(deviceType) {
				case DeviceType.CdRom:
				case DeviceType.CdRomFileSystem:
					return @$"\Device\CdRom{DeviceNumber}";
				default:
					throw new NotImplementedException();
			}
		}
	}

	enum DeviceType : ushort {
		CdRom = 2,
		CdRomFileSystem = 3,
		Disk = 7,
		DiskFileSystem = 8,
		Dvd = 0x033
	}
}
