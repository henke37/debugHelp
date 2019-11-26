using System;

namespace Henke37.Win32.BatteryAccess {
	internal struct BatteryWaitStatus {
		internal UInt64 BatteryTag;
		internal Int64 Timeout;
		internal PowerStateFlags PowerState;
		internal UInt64 LowCapacity;
		internal UInt64 HighCapacity;
	}
}