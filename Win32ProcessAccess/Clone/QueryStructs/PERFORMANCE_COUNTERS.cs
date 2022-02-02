﻿using Henke37.Win32.Processes;
using System;
using System.Runtime.InteropServices;

using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Henke37.Win32.Clone.QueryStructs {
	class PERFORMANCE_COUNTERS {

		public TimeSpan Total;
		public TimeSpan VaClone;
		public TimeSpan TotalWallClock;
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

			public PERFORMANCE_COUNTERS AsManaged() {
				return new PERFORMANCE_COUNTERS() {
					Total = ProcessTimes.FiletimeToTimeSpan(TotalWallClockPeriod),
					VaClone = ProcessTimes.FiletimeToTimeSpan(VaCloneWallClockPeriod),
					VaSpace = ProcessTimes.FiletimeToTimeSpan(VaSpaceWallClockPeriod),
					AuxPages = ProcessTimes.FiletimeToTimeSpan(AuxPagesWallClockPeriod),
					Handles = ProcessTimes.FiletimeToTimeSpan(HandlesWallClockPeriod),
					Threads = ProcessTimes.FiletimeToTimeSpan(ThreadsWallClockPeriod)
				};
			}
		}
	}
}
