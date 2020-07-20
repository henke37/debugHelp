using Henke37.Win32.Base;
using System;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32.Jobs {

	public class BasicLimitInformation {
		public TimeSpan PerProcessUserTimeLimit;
		public TimeSpan PerJobUserTimeLimit;

		public LimitFlags LimitFlags;

		public UInt32 MinimumWorkingSetSize;
		public UInt32 MaximumWorkingSetSize;

		public UInt32 ActiveProcessLimit;

		public UInt64 Affinity;

		public UInt32 PriorityClass;
		public UInt32 SchedulingClass;

		[StructLayout(LayoutKind.Sequential)]
		internal struct Native {
			internal LargeInteger PerProcessUserTimeLimit;
			internal LargeInteger PerJobUserTimeLimit;

			public LimitFlags LimitFlags;

			public UInt32 MinimumWorkingSetSize;
			public UInt32 MaximumWorkingSetSize;

			public UInt32 ActiveProcessLimit;

			public UInt64 Affinity;

			public UInt32 PriorityClass;
			public UInt32 SchedulingClass;

			internal Native(BasicLimitInformation managed) {
				PerProcessUserTimeLimit = new LargeInteger(managed.PerProcessUserTimeLimit.Ticks);
				PerJobUserTimeLimit = new LargeInteger(managed.PerJobUserTimeLimit.Ticks);
				LimitFlags = managed.LimitFlags;
				MinimumWorkingSetSize = managed.MinimumWorkingSetSize;
				MaximumWorkingSetSize = managed.MaximumWorkingSetSize;
				ActiveProcessLimit = managed.ActiveProcessLimit;
				Affinity = managed.Affinity;
				PriorityClass = managed.PriorityClass;
				SchedulingClass = managed.SchedulingClass;
			}

			public BasicLimitInformation AsManaged() {
				return new BasicLimitInformation() {
					PerProcessUserTimeLimit = TimeSpan.FromTicks(PerProcessUserTimeLimit.QuadPart),
					PerJobUserTimeLimit = TimeSpan.FromTicks(PerJobUserTimeLimit.QuadPart),
					LimitFlags = LimitFlags,
					MinimumWorkingSetSize = MinimumWorkingSetSize,
					MaximumWorkingSetSize = MaximumWorkingSetSize,
					ActiveProcessLimit = ActiveProcessLimit,
					Affinity = Affinity,
					PriorityClass = PriorityClass,
					SchedulingClass = SchedulingClass
				};
			}
		}
	}

	[Flags]
	public enum LimitFlags {
		None = 0,
		ActiveProcess = 0x00000008,
		Affinity = 0x00000010,
		BreakawayOK = 0x00000800,
		DieOnUnhandledException = 0x00000400,
		JobMemory = 0x00000200,
		JobTime = 0x00000004,
		KillOnJobClose = 0x00002000,
		PreserveJobTime = 0x00000040,
		PriorityClass = 0x00000020,
		ProcessMemory = 0x00000100,
		ProcessTime = 0x00000002,
		SchedulingClass = 0x00000080,
		SilentBreakawayOK = 0x00001000,
		SubsetAffinity = 0x00004000,
		WorkingSet = 0x00000001
	}
}
