using System;

namespace Henke37.Win32.AccessRights {
	[Flags]
	public enum JobAccessRights : uint {
		None = 0,
		Delete = 0x00010000,
		ReadControl = 0x00020000,
		Synchronize = 0x00100000,
		WriteDAC = 0x00040000,
		WriteOwner = 0x00080000,

		AssignProcess = 0x0001,
		Query = 0x0004,
		SetAttributes = 0x0002,
		Terminate = 0x0008,

		All = Delete | ReadControl | Synchronize |
			WriteDAC | WriteOwner |
			AssignProcess |
			Query | SetAttributes |
			Terminate
	}
}
