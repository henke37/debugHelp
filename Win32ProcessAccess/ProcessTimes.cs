using System;

namespace Henke37.DebugHelp.Win32 {
	public struct ProcessTimes {
		public DateTime CreationTime;
		public DateTime ExitTime;
		public TimeSpan KernelTime;
		public TimeSpan UserTime;
	}
}