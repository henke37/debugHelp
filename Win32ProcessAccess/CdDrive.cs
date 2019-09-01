using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Henke37.DebugHelp.Win32.AccessRights;

namespace Henke37.DebugHelp.Win32 {
	public class CdDrive {
		NativeFileObject file;

		public CdDrive(string path) {
			file = NativeFileObject.Open(path, FileObjectAccessRights.GenericRead|FileObjectAccessRights.GenericWrite, FileShareMode.Write|FileShareMode.Read, FileDisposition.OpenExisting, 0);
		}

		public void Eject() {
			file.DeviceControl(DeviceIoControlCode.DiskEjectMedia);
		}

		public void Load() {
			file.DeviceControl(DeviceIoControlCode.DiskLoadMedia);
		}
	}
}
