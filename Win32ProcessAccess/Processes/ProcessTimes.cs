using System;
using System.Runtime.InteropServices.ComTypes;

namespace Henke37.Win32.Processes {
	public struct ProcessTimes {
		public DateTime CreationTime;
		public DateTime ExitTime;
		public TimeSpan KernelTime;
		public TimeSpan UserTime;

		internal ProcessTimes(FILETIME creationTime, FILETIME exitTime, FILETIME kernelTime, FILETIME userTime) {
			CreationTime = FiletimeToDateTime(creationTime);
			ExitTime = FiletimeToDateTime(exitTime);
			KernelTime = FiletimeToTimeSpan(kernelTime);
			UserTime = FiletimeToTimeSpan(userTime);
		}

		internal static DateTime FiletimeToDateTime(FILETIME fileTime) {
			//NB! uint conversion must be done on both fields before ulong conversion
			ulong hFT2 = unchecked((((ulong)(uint)fileTime.dwHighDateTime) << 32) | (uint)fileTime.dwLowDateTime);
			return DateTime.FromFileTimeUtc((long)hFT2);
		}

		internal static TimeSpan FiletimeToTimeSpan(FILETIME fileTime) {
			//NB! uint conversion must be done on both fields before ulong conversion
			ulong hFT2 = unchecked((((ulong)(uint)fileTime.dwHighDateTime) << 32) | (uint)fileTime.dwLowDateTime);
			return TimeSpan.FromTicks((long)hFT2);
		}

		public void Deconstruct(out DateTime CreationTime, out DateTime ExitTime) {
			CreationTime = this.CreationTime;
			ExitTime = this.ExitTime;
		}
		public void Deconstruct(out TimeSpan KernelTime, out TimeSpan UserTime) {
			KernelTime = this.KernelTime;
			UserTime = this.UserTime;
		}
		public void Deconstruct(out DateTime CreationTime, out DateTime ExitTime, out TimeSpan KernelTime, out TimeSpan UserTime) {
			CreationTime = this.CreationTime;
			ExitTime = this.ExitTime;
			KernelTime = this.KernelTime;
			UserTime = this.UserTime;
		}
	}
}