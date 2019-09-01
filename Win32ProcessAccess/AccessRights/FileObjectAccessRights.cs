using System;

namespace Henke37.DebugHelp.Win32.AccessRights {
	[Flags]
	public enum FileObjectAccessRights : uint {
		None = 0,
		AppendData = 4,
		FileExecute = 32,
		ReadAttributes = 128,
		ReadData = 1,
		ReadExtendedAttributes = 8,
		WriteAttributes = 256,
		WriteData = 2,
		WriteExtendedAttributes = 16,

		GenericRead = 0x80000000,
		GenericWrite = 0x40000000,
		GenericExecute = 0x20000000,
		GenericAll = 0x10000000
	}

	public enum DirectoryAccessRights : uint {
		None = 0,
		AddFile = 2,
		AddSubdirectory = 4,
		DeleteChild = 64,
		ListDirectory = 1,
		Traverse = 32,
	}

	public enum PipeAccessRights : uint {
		CreatePipeInstance = 4,
	}
}
