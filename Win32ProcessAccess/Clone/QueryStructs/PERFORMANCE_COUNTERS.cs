using Henke37.Win32.Base;
using Henke37.Win32.Processes;
using System;
using System.Runtime.InteropServices;

using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Henke37.Win32.Clone.QueryStructs {
	public class PerformanceCounters {

		public TimeSpan Total;
		public TimeSpan VaClone;
		public TimeSpan VaSpace;
		public TimeSpan AuxPages;
		public TimeSpan Handles;
		public TimeSpan Threads;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct Native {
			UInt64 TotalCycleCount;
			FILETIME TotalWallClockPeriod;
			UInt64 VaCloneCycleCount;
			FILETIME VaCloneWallClockPeriod;
			UInt64 VaSpaceCycleCount;
			FILETIME VaSpaceWallClockPeriod;
			UInt64 AuxPagesCycleCount;
			FILETIME AuxPagesWallClockPeriod;
			UInt64 HandlesCycleCount;
			FILETIME HandlesWallClockPeriod;
			UInt64 ThreadsCycleCount;
			FILETIME ThreadsWallClockPeriod;

			public PerformanceCounters AsManaged() {
				return new PerformanceCounters() {
					Total = TotalWallClockPeriod.ToTimeSpan(),
					VaClone = VaCloneWallClockPeriod.ToTimeSpan(),
					VaSpace = VaSpaceWallClockPeriod.ToTimeSpan(),
					AuxPages = AuxPagesWallClockPeriod.ToTimeSpan(),
					Handles = HandlesWallClockPeriod.ToTimeSpan(),
					Threads = ThreadsWallClockPeriod.ToTimeSpan()
				};
			}
		}
	}
}
