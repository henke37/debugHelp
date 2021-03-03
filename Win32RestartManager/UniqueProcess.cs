using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Henke37.Win32.Restart {
	public class UniqueProcess {
		public UInt32 ProcessId;
		public DateTime StartTime;

		[StructLayout(LayoutKind.Sequential,CharSet = CharSet.Unicode)]
		internal struct Native {
#pragma warning disable CS0649
			UInt32 processId;
			System.Runtime.InteropServices.ComTypes.FILETIME startTime;
#pragma warning restore CS0649

			public UniqueProcess AsNative() {
				return new UniqueProcess() {
					ProcessId = processId,
					StartTime = DateTime.FromFileTime(
						(((long)startTime.dwHighDateTime) << 32) | ((uint)startTime.dwLowDateTime)
						)
				};
			}
		}

		public override string ToString() {
			return $"{ProcessId} {StartTime}";
		}
	}
}