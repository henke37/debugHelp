using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.DebugHelp.Win32 {
	[Flags]
	public enum ProcessAccessRights {
		None = 0,
		CreateProcess = 0x0080,
		CreateThread = 0x0002,
		DuplicateHandle = 0x0040,
		QueryInformation = 0x0400,
		QueryLimitedInformation = 0x1000,
		SetInformation = 0x0200,
		SetQuota = 0x0100,
		SuspendResume = 0x0800,
		VMOperation = 0x0008,
		VMRead = 0x0010,
		VMWrite = 0x0020,
		Synchronize = 0x00100000,
		ReadControl = 0x00020000,
		WriteDACL = 0x00040000,
		WriteOwner = 0x00080000,
		All = CreateProcess | CreateThread | DuplicateHandle |
			QueryInformation | QueryLimitedInformation |
			SetInformation | SetQuota |
			SuspendResume |
			VMOperation | VMRead | VMWrite |
			Synchronize |
			WriteDACL | WriteOwner | ReadControl
	}
}
