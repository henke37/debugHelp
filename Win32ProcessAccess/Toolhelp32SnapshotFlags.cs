using System;

namespace Henke37.DebugHelp.Win32 {
	[Flags]
	public enum Toolhelp32SnapshotFlags : uint {
		None=0,
		Inherit= 0x80000000,
		HeapList= 0x00000001,
		Module= 0x00000008,
		Module32= 0x00000010,
		Process= 0x00000002,
		Thread= 0x00000004
	}
}