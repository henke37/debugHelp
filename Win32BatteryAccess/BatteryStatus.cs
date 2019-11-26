using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.BatteryAccess {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BatteryStatus {
		public PowerStateFlags PowerState;
		public UInt64 Capacity;
		public UInt64 Voltage;
		public UInt64 Rate;

		public const UInt64 UnknownRate    = 0x80000000;
		public const UInt64 UnknownVoltage = 0xFFFFFFFF;
	}

	[Flags]
	public enum PowerStateFlags : UInt64 {
		None=0,
		Charging    = 0x00000004,
		Critical    = 0x00000008,
		Discharging = 0x00000002,
		PowerOnLine = 0x00000001
	}
}