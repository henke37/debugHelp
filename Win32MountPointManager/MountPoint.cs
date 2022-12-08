using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.MountPointManager {
	public class MountPoint {
		public string? SymbolicLinkName;
		public string? UniqueId;
		public string? DeviceName;

		internal struct Header {
			internal UInt32 SymLinkNameOffset;
			internal UInt16 SymLinkNameLength;
			internal UInt16 Padding1;

			internal UInt32 UniqueIdOffset;
			internal UInt16 UniqueIdLength;
			internal UInt16 Padding2;

			internal UInt32 DeviceNameOffset;
			internal UInt16 DeviceNameLength;
			internal UInt16 Padding3;
		}
	}
}
