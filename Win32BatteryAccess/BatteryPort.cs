using Henke37.Win32.Base;
using Henke37.Win32.Base.AccessRights;
using Henke37.Win32.DeviceEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Henke37.Win32.BatteryAccess {
	public class BatteryPort {
		internal NativeFileObject file;

		internal const UInt64 InvalidTag = 0;

		internal static readonly Guid BatteryGuid = new Guid(0x72631e54, 0x78A4, 0x11d0, 0xbc, 0xf7, 0x00, 0xaa, 0x00, 0xb7, 0xb3, 0x2a);

		public BatteryPort(DeviceInterface device, bool readOnly = true) : this(device.FilePath, readOnly) { }

		private BatteryPort(string path, bool readOnly = true) {
			var rights = readOnly ? FileObjectAccessRights.GenericRead : FileObjectAccessRights.GenericRead | FileObjectAccessRights.GenericWrite;
			var share= readOnly ? FileShareMode.Read : FileShareMode.Write | FileShareMode.Read;
			file = NativeFileObject.Open(path, rights, share, FileDisposition.OpenExisting, 0);
		}

		public UInt64 BatteryTag {
			get => GetBatteryTag();
		}

		private UInt64 GetBatteryTag() {
			UInt64 timeout = 0;
			UInt64 tagBuff= InvalidTag;
			file.DeviceControlInputOutput<UInt64, UInt64>(DeviceIoControlCode.BatteryQueryTag, ref timeout, ref tagBuff);

			return tagBuff;
		}

		public static IEnumerable<DeviceInterface> GetBatteries() {
			var devInfo = new DeviceInformationSet(BatteryGuid, DeviceInformationClassFlags.Present | DeviceInformationClassFlags.DeviceInterface);
			return devInfo.GetDevices();
		}

		public BatteryStatus GetStatus(UInt64 BatteryTag) {
			BatteryWaitStatus wait=new BatteryWaitStatus();
			wait.BatteryTag = BatteryTag;
			wait.Timeout = 0;
			BatteryStatus status;
			status = file.DeviceControlInputOutput<BatteryWaitStatus, BatteryStatus>(
				DeviceIoControlCode.BatteryQueryStatus,
				ref wait
			);
			return status;
		}

		public string GetSerialNumber(UInt64 BatteryTag) {
			return QueryInfoString(QueryInformationLevel.SerialNumber, BatteryTag);
		}

		public string GetManufactureName(UInt64 BatteryTag) {
			return QueryInfoString(QueryInformationLevel.ManufactureName, BatteryTag);
		}

		public DateTime GetManufactureDate(UInt64 BatteryTag) {
			throw new NotImplementedException();
		}

		public string GetUniqueID(UInt64 BatteryTag) {
			return QueryInfoString(QueryInformationLevel.UniqueID, BatteryTag);
		}

		public string GetDeviceName(UInt64 BatteryTag) {
			return QueryInfoString(QueryInformationLevel.DeviceName, BatteryTag);
		}

		public TimeSpan GetEstimatedTime(UInt64 BatteryTag) {
			throw new NotImplementedException();
		}

		public UInt64 GetTemperature(UInt64 BatteryTag) {
			throw new NotImplementedException();
		}


		private string QueryInfoString(QueryInformationLevel informationLevel, ulong batteryTag) {
			throw new NotImplementedException();
		}
	}
}
