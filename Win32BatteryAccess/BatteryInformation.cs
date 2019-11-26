using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Henke37.Win32.BatteryAccess {

	public class BatteryInformation {
		public BatteryCapabilitiesFlags Capabilities;
		public BatteryTechnology BatteryTechnology;
		public string Chemistry;
		public UInt64 DesignatedCapacity;
		public UInt64 FullyChargedCapacity;
		public UInt64 DefaultAlert1;
		public UInt64 DefaultAlert2;
		public UInt64 CriticalBias;
		public UInt64 CycleCount;

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal unsafe struct Native {
			internal BatteryCapabilitiesFlags Capabilities;
			internal BatteryTechnology BatteryTechnology;
			internal fixed byte Padding1[3];
			internal fixed sbyte Chemistry[4];
			internal UInt64 DesignatedCapacity;
			internal UInt64 FullyChargedCapacity;
			internal UInt64 DefaultAlert1;
			internal UInt64 DefaultAlert2;
			internal UInt64 CriticalBias;
			internal UInt64 CycleCount;

			public BatteryInformation AsNative() {
				string chemString;

				fixed(sbyte* chemP = Chemistry) {
					chemString = new string(chemP, 0, 4);
				}

				return new BatteryInformation() {
					Capabilities = Capabilities,
					BatteryTechnology = BatteryTechnology,
					Chemistry = chemString,
					DesignatedCapacity = DesignatedCapacity,
					FullyChargedCapacity = FullyChargedCapacity,
					DefaultAlert1 = DefaultAlert1,
					DefaultAlert2 = DefaultAlert2,
					CriticalBias = CriticalBias,
					CycleCount = CycleCount
				};
			}
		}
	}

	[Flags]
	public enum BatteryCapabilitiesFlags : UInt64 {
		CapacityRelative= 0x40000000,
		IsShortTerm= 0x20000000,
		SetChargeSupported= 0x00000001,
		SetDischargeSupported= 0x00000002,
		SystemBattery= 0x80000000
	}

	public enum BatteryTechnology : byte {
		NonRechargeable=0,
		Rechargable=1
	}
}
