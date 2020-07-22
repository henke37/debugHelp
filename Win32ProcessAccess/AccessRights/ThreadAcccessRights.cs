using System;

namespace Henke37.Win32.AccessRights {
	[Flags]
	public enum ThreadAcccessRights : UInt32 {
		None = 0,
		Terminate = 0x0001,
		SuspendResume = 0x0002,
		GetContext = 0x0008,
		SetContext = 0x0010,
		SetInformation = 0x0020,
		QueryInformation = 0x0040,
		SetThreadToken = 0x0080,
		Impersonate = 0x0100,
		DirectImpersonation = 0x0200,
		SetLimitedInformation = 0x0400,
		QueryLimitedInformation = 0x0800,
		Synchronize = 0x00100000,
		ReadControl = 0x00020000,
		WriteDACL = 0x00040000,
		WriteOwner = 0x00080000,

		All = Terminate | SuspendResume | GetContext | SetContext |
			SetInformation | QueryInformation |
			SetThreadToken | Impersonate | DirectImpersonation |
			Synchronize | ReadControl | WriteDACL | WriteOwner
	}
}