using System;

namespace Henke37.DebugHelp.Win32.Jobs {
	public struct NetRateControlInformation {
		public UInt64 MaxBandwidth;
		public NetRateControlFlags Flags;
		public byte DSCP;
	}

	[Flags]
	public enum NetRateControlFlags {
		None = 0,
		Enable = 1,
		MaxBandwidth = 2,
		DSCPTag = 4
	}
}
