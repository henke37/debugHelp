using Henke37.Win32.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			throw new NotImplementedException();
		}
	}
}
