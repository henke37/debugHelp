using System;

namespace Henke37.Win32.Tokens {
	[Flags]
	public enum GroupFlags : UInt32 {
		Enabled = 0x00000004,
		EnabledByDefault = 0x00000002,
		Integrity = 0x00000020,
		IntegrityEnabled = 0x00000040,
		LogonId = 0xC0000000,
		Mandatory = 0x00000001,
		Owner = 0x00000008,
		Resource = 0x20000000,
		ForDenyOnly = 0x00000010
	}
}
