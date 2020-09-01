using System;

namespace Henke37.Win32.Processes {
	public enum ProcThreadAttribute : UInt32 {
		GroupAffinity,
		HandleList,
		IdealProcessor,
		MitigationPolicy,
		ParentProcess,
		PreferredNode,
		UMSThread,
		SecurityCapabilities,
		ProtectionLevel,
		ChildProcessPolicy,
		DesktopAppPolicy
	}
}