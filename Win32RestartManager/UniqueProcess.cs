using System;
using System.Runtime.InteropServices.ComTypes;

namespace Henke37.Win32.Restart {
	public class UniqueProcess {
		public UInt32 ProcessId;
		public DateTime StartTime;

		internal struct Native {
			UInt32 processId;
			FILETIME startTime;

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