using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.BatteryAccess {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct QueryInformation {
		internal UInt64 BatteryTag;
		internal QueryInformationLevel InformationLevel;
		internal Int64 AtRate;

		public QueryInformation(ulong batteryTag, QueryInformationLevel informationLevel) : this() {
			BatteryTag = batteryTag;
			InformationLevel = informationLevel;
			AtRate = 0;
		}
	}

	internal enum QueryInformationLevel : UInt64 {
		DeviceName=4,
		EstimatedTime=3,
		GranularityInformation=1,
		Information=0,
		ManufactureDate=5,
		ManufactureName=6,
		SerialNumber=8,
		Temperature=2,
		UniqueID=7
	}
}
