using Henke37.Win32.Base;
using System;

namespace Henke37.Win32.Threads {
	[Undocumented]
	internal enum ThreadInformationClass : UInt32 {
		BasicInformation = 0,
		Times = 1,
		Priority = 2,
		BasePriority = 3,
		AffinityMask = 4,
		ImpersonationToken = 5,
		DescriptorTableEntry = 6,
		EnagleAlignmentFaultFixup = 7,
		Win32StartAddress = 0x09,
		ZeroTlsCell = 0x0A,
		PerformanceCount = 0x0B,
		AmILastThread = 0x0C,
		IdealProcessor = 0x0D,
		PriorityBoost = 0x0E,
		IsIOPending = 0x010,
		HideFromDebugger = 0x011,
		BreakOnTermination = 0x012,
		IsTerminated = 0x014,
		LastSystemCall = 0x015,
		IOPriority = 0x016,
		CycleTime = 0x017,
		PagePriority = 0x018,
		ActualBasePriority = 0x019,
		TebInformation = 0x01A,
		WOW64Context = 0x01D,
		GroupInformation = 0x01E,
		UmsInformation = 0x01F,
		CounterProfiling = 0x020,
		IdealProcessorEx = 0x021,
		CpuAccountingInformation = 0x022,
		SuspendCount = 0x023,
		HeterogeneousCpuPolicy = 0x024,
		ContainerId = 0x025,
		NameInformation = 0x026,
		SelectedCpuSets = 0x027,
		SystemThreadInformation = 0x028,
		ActualGroupAffinity = 0x029
	}
}