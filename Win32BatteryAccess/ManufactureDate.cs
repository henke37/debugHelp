using System.Runtime.InteropServices;

namespace Henke37.Win32.BatteryAccess {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct ManufactureDate {
		public byte Day;
		public byte Month;
		public short Year;
	}
}