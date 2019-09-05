using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.CdAccess {
	public class TOC {
		public byte FirstTrack;
		public byte LastTrack;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal unsafe struct Native {
			UInt16 length;
			byte FirstTrack;
			byte LastTrack;
			//fixed TrackEntry.Native tracks;

			internal TOC AsManaged() {
				return new TOC() {
					FirstTrack=FirstTrack,
					LastTrack=LastTrack
				};
			}
		}
	}

	public class TrackEntry {

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal unsafe struct Native {
		}
	}
}