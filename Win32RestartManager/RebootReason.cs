using System;

namespace Henke37.Win32.Restart {
	[Flags]
	public enum RebootReason {
		None,
		PermissionDenied = 0x1,
		SessionMismatch = 0x2,
		CriticalProcess = 0x4,
		CriticalService = 0x8,
		DetectedSelf = 0x10
	}
}