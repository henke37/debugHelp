using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.BatteryAccess {
	internal struct QueryInformation {
		internal UInt64 BatteryTag;
		internal QueryInformationLevel InformationLevel;
		internal Int64 AtRate;
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
