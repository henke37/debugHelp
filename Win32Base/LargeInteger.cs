using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32 {
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public struct LargeInteger {
		[FieldOffset(0)] public Int64 QuadPart;
		[FieldOffset(0)] public UInt32 LowPart;
		[FieldOffset(4)] public Int32 HighPart;

		public LargeInteger(Int64 QVal) {
			LowPart = 0;
			HighPart = 0;
			QuadPart = QVal;
		}

		public LargeInteger(UInt32 low, Int32 high) {
			QuadPart = 0;
			LowPart = low;
			HighPart = high;
		}
	}
}
