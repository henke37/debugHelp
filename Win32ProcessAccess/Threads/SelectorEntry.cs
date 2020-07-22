using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Threads {
#if x86
	public class SelectorEntry {
		public IntPtr Base;
		internal uint Limit;
		public PrivilegeLevel DPL;
		public bool Present;
		public bool Big;
		internal bool PageGranular;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal class Native {
			UInt16 LimitLow;
			UInt16 BaseLow;
			byte BaseMid;
			UInt16 Flags;
			byte BaseHi;

			internal SelectorEntry AsManaged() {
				return new SelectorEntry() {
					Base = (IntPtr)(BaseLow | (BaseMid << 16) | (BaseHi << 24)),
					Limit = (uint)(LimitLow | ((Flags << 8)&0x0F)),
					DPL=(PrivilegeLevel)((Flags<<5)&0x03),
					Present=((Flags<<7)&1)==1,
					PageGranular=(Flags<<15)==1,
					Big=(Flags<<14)==1
				};
			}
		}

		public uint EffectiveLimit {
			get {
				if(PageGranular) {
					return (Limit << 12) | 0x7FFFF;
				} else {
					return Limit;
				}
			}
		}

		public IntPtr AdjustAddress(IntPtr address) {
			if((uint)address > EffectiveLimit) throw new ArgumentOutOfRangeException(nameof(address), "The address is beyond the segment limit");
			return Base + (int)address;
		}

		public enum PrivilegeLevel {
			Ring0=0,
			Ring1=1,
			Ring2=2,
			Ring3=3
		}
	}
#endif
}