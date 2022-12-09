using Henke37.Win32.Files;
using System;
using System.Collections.Generic;
using System.Text;

namespace Henke37.Win32.MountPointManager {
	public class MountPointManager {
		private NativeFileObject file;

		public MountPointManager() {
			file = NativeFileObject.Open(
				"\\\\.\\MountPointManager", 
				AccessRights.FileObjectAccessRights.GenericRead,
				FileShareMode.Read,
				FileDisposition.OpenExisting, 
				FileAttributes.Normal
			);
		}

		public List<MountPoint> QueryPoints(MountPoint needle) {
			var buff=needle.ToBuff();
			throw new NotImplementedException();
		}
	}
}
