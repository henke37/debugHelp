using System;
using System.Runtime.InteropServices.ComTypes;

namespace Henke37.Win32.Restart {
	public class UniqueProcess {
		UInt32 processId;
		DateTime startTime;

		internal struct Native {
			UInt32 processId;
			FILETIME startTime;

			public UniqueProcess AsNative() {
				return new UniqueProcess() {
					processId = processId,
					startTime = DateTime.FromFileTimeUtc(
						(((long)startTime.dwHighDateTime) << 32) | ((uint)startTime.dwLowDateTime)
						)
				};
			}
		}
	}
}