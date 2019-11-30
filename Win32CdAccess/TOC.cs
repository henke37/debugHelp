using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Henke37.Win32.CdAccess {
	public class TOC {
		public byte FirstTrack;
		public byte LastTrack;
		internal List<TrackEntry> Tracks;

		public TOC(byte firstTrack, byte lastTrack, List<TrackEntry> tracks) {
			FirstTrack = firstTrack;
			LastTrack = lastTrack;
			Tracks = tracks ?? throw new ArgumentNullException(nameof(tracks));
		}

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

		public enum TrackAddr {
			NoData=0,
			Position=1,
			MediaCatalogNumber=2,
			ISRC=3
		}

		public enum TrackCtrl {
			AudioNoPreEmphasis=0,
			AudioWithPreEmphasis=1,
			Data =4,
			DataIncremental=5,

		}
	}
}