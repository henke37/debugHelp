using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Jobs {
	public class CpuRateControlInformation {
		public CpuRateFlags ControlFlags;
		public UInt32 CpuRate;
		public UInt32 Weight;
		public UInt16 MinRate;
		public UInt16 MaxRate;

		public CpuRateControlInformation() { }
		internal CpuRateControlInformation(Native native) {
			ControlFlags = native.ControlFlags;
			if((ControlFlags | CpuRateFlags.WeightBased) != 0) {
				Weight = native.Weight;
			} else if((ControlFlags | CpuRateFlags.HardCap) != 0) {
				CpuRate = native.CpuRate;
			} else if((ControlFlags | CpuRateFlags.MinMaxRate) != 0) {
				MinRate = native.MinRate;
				MaxRate = native.MaxRate;
			}
		}

		[StructLayout(LayoutKind.Explicit, Size = 8, Pack = 1)]
		internal struct Native {
			[FieldOffset(0)]
			public CpuRateFlags ControlFlags;
			[FieldOffset(4)]
			public UInt32 CpuRate;
			[FieldOffset(4)]
			public UInt32 Weight;
			[FieldOffset(4)]
			public UInt16 MinRate;
			[FieldOffset(6)]
			public UInt16 MaxRate;

			public Native(CpuRateControlInformation managed) {
				ControlFlags = managed.ControlFlags;
				CpuRate = 0;
				Weight = 0;
				MinRate = 0;
				MaxRate = 0;
				if((ControlFlags | CpuRateFlags.WeightBased)!=0) {
					Weight = managed.Weight;
				} else if((ControlFlags | CpuRateFlags.HardCap)!=0) {
					CpuRate = managed.CpuRate;
				} else if((ControlFlags | CpuRateFlags.MinMaxRate)!=0) {
					MinRate = managed.MinRate;
					MaxRate = managed.MaxRate;
				}
			}
		}

	}

	[Flags]
	public enum CpuRateFlags : UInt32 {
		None = 0,
		Enable = 0x1,
		WeightBased = 0x2,
		HardCap = 0x4,
		Notify = 0x8,
		MinMaxRate = 0x10
	}
}
