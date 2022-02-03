using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Henke37.Win32.Base {
	public static class FileTimeExtensions {
		public static DateTime ToDateTime(this FILETIME fileTime) {
			//NB! uint conversion must be done on both fields before ulong conversion
			ulong hFT2 = unchecked((((ulong)(uint)fileTime.dwHighDateTime) << 32) | (uint)fileTime.dwLowDateTime);
			return DateTime.FromFileTimeUtc((long)hFT2);
		}

		public static TimeSpan ToTimeSpan(this FILETIME fileTime) {
			//NB! uint conversion must be done on both fields before ulong conversion
			ulong hFT2 = unchecked((((ulong)(uint)fileTime.dwHighDateTime) << 32) | (uint)fileTime.dwLowDateTime);
			return TimeSpan.FromTicks((long)hFT2);
		}
	}
}
