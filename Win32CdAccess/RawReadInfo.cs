using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.CdAccess {

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	struct RawReadInfo {
		public UInt64 DiskOffset;
		public UInt32 SectorCount;
		public TrackReadMode ReadMode;
	}

	public enum TrackReadMode : UInt32 {
		YellowMode2,
		XAForm2,
		CDDA,
		RawWithC2AndSubCode,
		RawWithC2,
		RawWithSubCode
	}

	internal enum RawReadSize : UInt32 {
		SectorWithC2           = 2352 + 296,
		SectorWithSubcode      = 2352       +96,
		SectorWithSubcodeAndC2 = 2352 + 296 + 96

	}
}
