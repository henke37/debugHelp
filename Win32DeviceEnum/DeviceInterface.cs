using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.DeviceEnum {
	public class DeviceInterface {
		public Guid InterfaceClassGuid;
		public DeviceInterfaceFlags Flags;
		public string FilePath;

		internal struct Native {
			internal UInt32 cdSize;
			internal Guid InterfaceClassGuid;
			internal DeviceInterfaceFlags Flags;
			internal UIntPtr Reserved;

			public DeviceInterface AsManaged(string filePath) {
				var di = new DeviceInterface() { 
					InterfaceClassGuid = this.InterfaceClassGuid,
					Flags = this.Flags,
					FilePath=filePath
				};

				return di;
			}
		}

		internal struct DetailsNative {
			internal UInt32 cbSize;
		}
	}

	[Flags]
	public enum DeviceInterfaceFlags {
		Active  = 0x00000001,
		Default = 0x00000002,
		Removed = 0x00000004
	}
}
