using System;
using Henke37.Win32.Base;
using Henke37.Win32.Base.AccessRights;

namespace Henke37.Win32.CdAccess {
	public class CdDrive {
		internal NativeFileObject file;

		public CdDrive(string path) {
			file = NativeFileObject.Open(path, FileObjectAccessRights.GenericRead|FileObjectAccessRights.GenericWrite, FileShareMode.Write|FileShareMode.Read, FileDisposition.OpenExisting, 0);
		}

		public void Eject() {
			file.DeviceControl(DeviceIoControlCode.DiskEjectMedia);
		}

		public void Load() {
			file.DeviceControl(DeviceIoControlCode.DiskLoadMedia);
		}

		public TOC GetTOC() {
			TOC.Native native;
			file.DeviceControlOutput<TOC.Native>(DeviceIoControlCode.CdRomReadTOC,ref native);
			return native.AsManaged();
		}

		public RegionData GetRegionData() {
			RegionData.Native native=new RegionData.Native();
			file.DeviceControlOutput<RegionData.Native>(DeviceIoControlCode.DvdGetRegion, ref native);
			return native.AsManaged();
		}
	}
}
