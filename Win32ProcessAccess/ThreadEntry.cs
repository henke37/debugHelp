using Henke37.DebugHelp.Win32.AccessRights;
using System;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
	public class ThreadEntry {
		public UInt32 ThreadId;
		public UInt32 ProcessId;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct Native {
			internal UInt32 dwSize;
			UInt32 cntUsage;
			UInt32 th32ThreadID;
			UInt32 th32OwnerProcessID;
			Int32 tpBasePri;
			Int32 tpDeltaPri;
			UInt32 dwFlags;

			internal ThreadEntry AsManaged() {
				return new ThreadEntry() {
					ThreadId = th32ThreadID,
					ProcessId = th32OwnerProcessID
				};
			}
		}

		public NativeThread Open(ThreadAcccessRights rights = ThreadAcccessRights.All, bool inheritable = false) {
			return NativeThread.Open(ThreadId, rights, inheritable);
		}

		public override string ToString() {
			return $"{ThreadId}";
		}
	}
}