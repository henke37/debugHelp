using System;

namespace Henke37.Win32.Processes {
	public enum ProcThreadAttribute : UInt32 {
		ParentProcess = 0 | 0x00020000,
		HandleList = 2 | 0x00020000,
		GroupAffinity = 3 | 0x00010000 | 0x00020000,
		PreferredNode = 4 | 0x00020000,
		IdealProcessor = 5 | 0x00010000 | 0x00020000,
		UMSThread = 6 | 0x00010000 | 0x00020000,
		MitigationPolicy = 7 | 0x00020000,
		SecurityCapabilities = 9 | 0x00020000,
		ProtectionLevel = 11 | 0x00020000,
		JobList = 13 | 0x00020000,
		ChildProcessPolicy = 14 | 0x00020000,
	}
}