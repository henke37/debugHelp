using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.BatteryAccess {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct BatteryWaitStatus {
		internal UInt64 BatteryTag;
		internal Int64 Timeout;
		internal PowerStateFlags PowerState;
		internal UInt64 LowCapacity;
		internal UInt64 HighCapacity;
	}
}