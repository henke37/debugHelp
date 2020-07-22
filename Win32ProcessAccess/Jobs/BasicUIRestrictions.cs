using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Jobs {
	[Flags]
	public enum BasicUIRestrictions {
		None = 0,
		Desktop = 0x00000040,
		DisplaySettings = 0x00000010,
		ExitWindows = 0x00000080,
		GlobalAtoms = 0x00000020,
		Handles = 0x00000001,
		ReadClipboard = 0x00000002,
		SystemParamerers = 0x00000008,
		WriteClipboard = 0x00000004
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct BasicUIRestrictionsStruct {
		public BasicUIRestrictions restrictions;

		public BasicUIRestrictionsStruct(BasicUIRestrictions value) : this() {
			this.restrictions = value;
		}
	}
}
