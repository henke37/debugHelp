using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.CdAccess {

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct ReadTocEx {
		internal byte FormatMsf;
		internal byte SessionTrack;
		byte Reserved1;
		byte Reserved2;

		public ReadTocEx(ReadTocFormat readFormat, bool useMsf) {
			FormatMsf = MakeFmtMsf(readFormat, useMsf);
			SessionTrack = 0;
			Reserved1 = 0;
			Reserved2 = 0;
		}

		public ReadTocEx(ReadTocFormat readFormat, bool useMsf, int sessionTrack) {
			FormatMsf = MakeFmtMsf(readFormat, useMsf);
			SessionTrack = (byte)sessionTrack;
			Reserved1 = 0;
			Reserved2 = 0;
		}

		private static byte MakeFmtMsf(ReadTocFormat readFormat, bool useMsf) {
			return (byte)(
				((byte)readFormat) | (useMsf?0x80:0x00)
			);
		}

		public ReadTocFormat Format => (ReadTocFormat)(FormatMsf &0x0F);
		public bool UseMsf => (FormatMsf & 0x80) != 0;
	}

	internal enum ReadTocFormat : byte {
		Toc = 0,
		Session=1,
		FullToc=2,
		PMA=3,
		ATIP=4,
		CDText=5
	}

	public class FullToc {
		public byte FirstCompleteSession;
		public byte LastCompleteSession;
		public List<TocFullDataBlock> Entries;

		internal FullToc(byte firstCompleteSession, byte lastCompleteSession, List<TocFullDataBlock> entries) {
			FirstCompleteSession = firstCompleteSession;
			LastCompleteSession = lastCompleteSession;
			Entries = entries ?? throw new ArgumentNullException(nameof(entries));
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct FullTocHeader {
			internal short Length;
			internal byte FirstCompleteSession;
			internal byte LastCompleteSession;
		}
	}

	public class TocFullDataBlock {
		public byte SessionNumber;
		public byte TrackNumber;
		public TrackAddr Addr;
		public TrackCtrl Ctrl;
		public byte Point;
		public TrackTime ATime;
		public TrackTime StartPosition;

		public TocFullDataBlock(byte sessionNumber, byte CtrlAddr, byte trackNumber, byte point, TrackTime aTime, TrackTime startPosition) {
			SessionNumber = sessionNumber;
			TrackNumber = trackNumber;
			Point = point;
			ATime = aTime;
			StartPosition = startPosition;

			Ctrl = (TrackCtrl)(CtrlAddr & 0x0F);
			Addr = (TrackAddr)((CtrlAddr >> 4) & 0x0F);
		}

		private TrackCtrl TypeCtrl => Ctrl & ~TrackCtrl.DigitalCopyAllowed;

		public bool IsData => TypeCtrl == TrackCtrl.Data || TypeCtrl == TrackCtrl.DataIncremental;
		public bool IsStereoAudio => TypeCtrl == TrackCtrl.StereoAudioNoPreEmphasis || TypeCtrl == TrackCtrl.StereoAudioWithPreEmphasis;
		public bool IsQuadAudio => TypeCtrl == TrackCtrl.QuadAudioNoPreEmphasis || TypeCtrl == TrackCtrl.QuadAudioWithPreEmphasis;
		public bool IsAudio => IsStereoAudio || IsQuadAudio;
		public bool IsDigitalCopyAllowed => (Ctrl & TrackCtrl.DigitalCopyAllowed) != 0;
		public bool HasPreEmphasis => TypeCtrl == TrackCtrl.StereoAudioWithPreEmphasis || TypeCtrl == TrackCtrl.QuadAudioWithPreEmphasis;

		public override string ToString() {
			return $"{TypeCtrl} {Point} {ATime} {StartPosition}";
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct Native {
			internal byte SessionNumber;
			internal byte CtrlAddr;
			internal byte TrackNumber;
			internal byte Point;
			internal byte Minutes;
			internal byte Seconds;
			internal byte Frames;
			internal byte Zero;

			internal byte PMinutes;
			internal byte PSeconds;
			internal byte PFrames;

			public TocFullDataBlock AsManaged() {
				return new TocFullDataBlock(SessionNumber,
					CtrlAddr,
					TrackNumber,
					Point,
					new TrackTime() {
						Minutes = Minutes,
						Seconds = Seconds,
						Frames = Frames
					},
					new TrackTime() {
						Minutes = PMinutes,
						Seconds = PSeconds,
						Frames = PFrames
					}
				);
			}
		}
	}
}
