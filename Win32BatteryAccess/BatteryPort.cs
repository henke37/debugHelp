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

		public string GetUniqueID(UInt64 BatteryTag) {
			return QueryInfoString(QueryInformationLevel.UniqueID, BatteryTag);
		}

		public string GetDeviceName(UInt64 BatteryTag) {
			return QueryInfoString(QueryInformationLevel.DeviceName, BatteryTag);
		}

		public TimeSpan GetEstimatedTime(UInt64 BatteryTag) {
			var secs=QueryInformation<UInt64>(QueryInformationLevel.EstimatedTime, BatteryTag);
			return new TimeSpan(0, 0, (int)secs);
		}

		public UInt64 GetTemperature(UInt64 BatteryTag) {
			return QueryInformation<UInt64>(QueryInformationLevel.Temperature, BatteryTag);
		}

		public DateTime GetManufactureDate(UInt64 BatteryTag) {
			var natDate = QueryInformation<ManufactureDate>(QueryInformationLevel.ManufactureDate, BatteryTag);
			return new DateTime(natDate.Year, natDate.Month, natDate.Day);
		}

		public BatteryInformation GetBatteryInformation(UInt64 BatteryTag) {
			var natInfo = QueryInformation<BatteryInformation.Native>(QueryInformationLevel.Information, BatteryTag);
			return natInfo.AsNative();
		}


		private T QueryInformation<T>(QueryInformationLevel informationLevel, ulong batteryTag) where T : unmanaged {
			QueryInformation query = new QueryInformation(batteryTag,informationLevel);
			return file.DeviceControlInputOutput<QueryInformation, T>(DeviceIoControlCode.BatteryQueryInformation, ref query);
		}

		private string QueryInfoString(QueryInformationLevel informationLevel, ulong batteryTag, int buffSize=128) {
			QueryInformation query = new QueryInformation(batteryTag, informationLevel);

			byte[] strBuff=new byte[buffSize];

			var written=file.DeviceControlInputOutput<QueryInformation>(DeviceIoControlCode.BatteryQueryInformation, ref query, strBuff);

			return Encoding.UTF8.GetString(strBuff, 0, (int)written);
		}
	}
}
