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
		public byte TrackNumber;
		public uint StartAddr;
		public TrackAddr Addr;
		public TrackCtrl Ctrl;

		internal TrackEntry(byte trackNumber, uint startAddr, byte CtrlAddr) {
			TrackNumber = trackNumber;
			StartAddr = startAddr;
			Ctrl = (TrackCtrl)(CtrlAddr & 0x0F);
			Addr = (TrackAddr)((CtrlAddr >> 4) & 0x0F);
		}

		public override string ToString() {
			TrackTime trackTime = TrackTime.FromFrames((int)StartAddr);
			return String.Format("{0} {1} {2}", TrackNumber, IsAudio ? "Audio" : "Data", trackTime);
		}

		private TrackCtrl TypeCtrl => Ctrl & ~TrackCtrl.DigitalCopyAllowed;

		public bool IsData => TypeCtrl == TrackCtrl.Data || TypeCtrl == TrackCtrl.DataIncremental;
		public bool IsStereoAudio => TypeCtrl == TrackCtrl.StereoAudioNoPreEmphasis || TypeCtrl == TrackCtrl.StereoAudioWithPreEmphasis;
		public bool IsQuadAudio => TypeCtrl == TrackCtrl.QuadAudioNoPreEmphasis || TypeCtrl == TrackCtrl.QuadAudioWithPreEmphasis;
		public bool IsAudio => IsStereoAudio || IsQuadAudio;
		public bool IsDigitalCopyAllowed => (Ctrl & TrackCtrl.DigitalCopyAllowed) != 0;
		public bool HasPreEmphasis => TypeCtrl == TrackCtrl.StereoAudioWithPreEmphasis || TypeCtrl == TrackCtrl.QuadAudioWithPreEmphasis;

		public bool IsLeadOut => TrackNumber == 0xAA;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal unsafe struct Native {
			byte Reserved;
			byte CtrlAdr;
			byte TrackNumber;
			byte Reserved2;
			UInt32 StartAddr;

			internal TrackEntry AsNative() {
				return new TrackEntry(TrackNumber, StartAddr, CtrlAdr);
			}
		}
	}

	public enum TrackAddr {
		NoData=0,
		Position=1,
		MediaCatalogNumber=2,
		ISRC=3
	}

	[Flags]
	public enum TrackCtrl {
		StereoAudioNoPreEmphasis=0,
		StereoAudioWithPreEmphasis=1,
		QuadAudioNoPreEmphasis = 8,
		QuadAudioWithPreEmphasis = 9,
		Data =4,
		DataIncremental=5,
		DigitalCopyAllowed=2
	}


	public struct TrackTime {
		public byte Minutes;
		public byte Seconds;
		public byte Frames;

		public const int FramesPerSec = 75;

		public static TrackTime FromFrames(int frames) {
			int secs = frames / FramesPerSec;
			return new TrackTime() {
				Minutes = (byte)(secs / 60),
				Seconds = (byte)(secs % 60),
				Frames = (byte)(frames % FramesPerSec),
			};
		}

		public override string ToString() {
			return $"{Minutes:D2}:{Seconds:D2}:{Frames:D2}";
		}
	}
	
}