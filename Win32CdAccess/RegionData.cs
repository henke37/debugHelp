using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.CdAccess {
	public class RegionData {
		public bool Protected;
		public Regions DiskRegion;
		public Regions SystemRegion;
		public byte ResetCount;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal unsafe struct Native {
			IntPtr handle;
			byte CopySystem;
			byte RegionData;
			byte SystemRegion;
			byte ResetCount;

			public RegionData AsManaged() {
				return new RegionData() {
					Protected=CopySystem!=0,
					DiskRegion=(Regions)RegionData,
					SystemRegion=(Regions)SystemRegion,
					ResetCount=ResetCount
				};
			}
		}

		[Flags]
		public enum Regions {
			America=1,
			EuropeAfrica=1<<1,
			Asia=1<<2,
			AustraliaSouthAmerica=1<<3,
			EasternEurope=1<<4,
			China=1<<5,
			SpecialInternational=1<<7
		}
	}
}