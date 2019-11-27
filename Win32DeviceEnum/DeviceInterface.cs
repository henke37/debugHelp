using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.DeviceEnum {
	public class DeviceInterface {
		public Guid InterfaceClassGuid;
		public DeviceInterfaceFlags Flags;
		public string FilePath;

		public DeviceInterface(Guid interfaceClassGuid, DeviceInterfaceFlags flags, string filePath) {
			InterfaceClassGuid = interfaceClassGuid;
			Flags = flags;
			FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct Native {
			internal UInt32 cdSize;
			internal Guid InterfaceClassGuid;
			internal DeviceInterfaceFlags Flags;
			internal UIntPtr Reserved;

			public DeviceInterface AsManaged(string filePath) {
				return new DeviceInterface(InterfaceClassGuid, Flags, filePath);
			}
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
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
