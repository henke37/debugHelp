using System;

namespace Henke37.Win32.Threads {
	public enum ThreadPriority : Int32 {
		TimeCritical = 15,
		Highest = 2,
		AboveNormal = 1,
		Normal = 0,
		BelowNormal = -1,
		Lowest = -2,
		Idle = -15
	}
}