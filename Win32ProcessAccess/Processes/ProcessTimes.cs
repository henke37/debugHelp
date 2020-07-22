using System;

namespace Henke37.Win32.Processes {
	public struct ProcessTimes {
		public DateTime CreationTime;
		public DateTime ExitTime;
		public TimeSpan KernelTime;
		public TimeSpan UserTime;
	}
}