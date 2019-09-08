using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Henke37.Win32.CdAccess {
	public class TOC {
		public byte FirstTrack;
		public byte LastTrack;
		internal List<TrackEntry> Tracks;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal unsafe struct TocHeader {
			UInt16 length;
			internal byte FirstTrack;
			internal byte LastTrack;
			//fixed TrackEntry.Native tracks;
		}
	}

	public class TrackEntry {
		public uint StartAddr;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal unsafe struct Native {

			byte Reserved;
			byte CtrlAdr;
			byte TrackNumber;
			byte Reserved2;
			UInt32 StartAddr;

			internal TrackEntry AsNative() {
				return new TrackEntry() { StartAddr = StartAddr };
			}
		}
	}
}