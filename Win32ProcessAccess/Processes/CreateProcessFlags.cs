using System;

namespace Henke37.Win32.Processes {
	[Flags]
	public enum CreateProcessFlags : UInt32 {
		None = 0,
		BreakAwayFromJob = 0x01000000,
		DefaultErrorMode = 0x04000000,
		CreateNewConsole = 0x00000010,
		CreateNewProcessGroup = 0x00000200,
		CreateNoWindow = 0x08000000,
		CreateProtectedProcess = 0x00040000,
		PreserveCodeAuthZLevel = 0x02000000,
		SecureProcess = 0x00400000,
		SeparateWOWVDM = 0x00000800,
		SharedWOWVDM = 0x00001000,
		Suspended = 0x00000004,
		UnicodeEnvironment = 0x00000400,
		DebugOnlyThisProcess = 0x00000002,
		DebugProcess = 0x00000001,
		DetachedProcess = 0x00000008,
		ExtendedStartupInfoPresent = 0x00080000,
		InheritParentAffinity = 0x00010000
	}
}
