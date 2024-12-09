using System;

namespace Henke37.Win32.Threads {
	public struct ProcessorNumber {
		public UInt16 Group;
		public byte Number;
		internal byte Reserved;

		public ProcessorNumber(byte Number) {
			Group = 0;
			this.Number = Number;
			Reserved = 0;
		}
		public ProcessorNumber(UInt16 Group, byte Number) {
			this.Group = Group;
			this.Number = Number;
			Reserved = 0;
		}
	}
}