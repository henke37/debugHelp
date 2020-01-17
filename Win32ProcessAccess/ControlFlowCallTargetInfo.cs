using System;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct ControlFlowCallTargetInfo {
		public int Offset;
		public ControlFlowCallTargetFlags Flags;
	}

	[Flags]
	public enum ControlFlowCallTargetFlags : uint {
		None = 0,
		Valid = 0x00000001
	}
}