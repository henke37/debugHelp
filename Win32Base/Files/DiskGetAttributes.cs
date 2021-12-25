using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Files {
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct DiskGetAttributes {
		internal UInt32 Version;
		UInt32 Reserved;
		internal UInt64 Attributes;
	}

	[Flags]
	internal enum DiskAttribute : UInt64 {
		None = 0,
		Offline = 1,
		ReadOnly = 2
	}
}