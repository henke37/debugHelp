using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.CdAccess {
	public class SessionData {
		public byte FirstCompleteSession;
		public byte LastCompleteSession;

		internal SessionData(byte firstCompleteSession, byte lastCompleteSession) {
			FirstCompleteSession = firstCompleteSession;
			LastCompleteSession = lastCompleteSession;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct Native {
			UInt16 length;
			byte FirstCompleteSession;
			byte LastCompleteSession;
			TrackEntry.Native FirstTrackInLastSession;

			internal SessionData AsManaged() {
				return new SessionData(FirstCompleteSession, LastCompleteSession);
			}
		}
	}
}