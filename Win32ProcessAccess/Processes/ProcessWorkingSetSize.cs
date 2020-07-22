using System;

namespace Henke37.Win32.Processes {
	public class ProcessWorkingSetSize {
		public uint Min;
		public uint Max;
		public QuotaFlags Flags;

		internal ProcessWorkingSetSize(uint min, uint max, uint flags) {
			this.Min = min;
			this.Max = max;
			this.Flags = (QuotaFlags)flags;
		}

		[Flags]
		public enum QuotaFlags : uint {
			MinDisable=2,
			MinEnable=1,
			MaxDisable=8,
			MaxEnable=4
		}
	}
}