using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.CdAccess {
	public class TOC {
		public byte FirstTrack;
		public byte LastTrack;

		private const int MAXIMUM_NUMBER_TRACKS = 100;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal unsafe struct Native {
			UInt16 length;
			byte FirstTrack;
			byte LastTrack;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXIMUM_NUMBER_TRACKS)]
			TrackEntry.Native[] tracks;

			internal TOC AsManaged() {
				return new TOC() {
					FirstTrack=FirstTrack,
					LastTrack=LastTrack
				};
			}
		}
	}

	class TrackEntry {

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal unsafe struct Native {
		}
	}
}