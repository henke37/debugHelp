using Henke37.Win32.AccessRights;
using Henke37.Win32.Threads;
using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Snapshots {
	public class ThreadEntry {
		public UInt32 ThreadId;
		public UInt32 ProcessId;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct Native {
			internal UInt32 dwSize;
			UInt32 cntUsage;
			internal UInt32 th32ThreadID;
			internal UInt32 th32OwnerProcessID;
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

		public NativeThread Open(ThreadAccessRights rights = ThreadAccessRights.All, bool inheritable = false) {
			return NativeThread.Open(ThreadId, rights, inheritable);
		}

		public override string ToString() {
			return $"{ThreadId}";
		}

		public void Deconstruct(out UInt32 ThreadId, out UInt32 ProcessId) {
			ThreadId = this.ThreadId;
			ProcessId = this.ProcessId;
		}
	}
}