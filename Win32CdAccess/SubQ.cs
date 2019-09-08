using System.Runtime.InteropServices;

namespace Henke37.Win32.CdAccess {

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct SubQDataFormat {
		internal byte Format;
		internal byte Track;
	}

	internal enum SubQDataFormatFormat {
		CurrentPosition=1,
		MediaCatalog=2,
		TrackISRC=3
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct SubQHeader {
		byte Reserved;
		internal byte AudioStatus;
		byte DataLenHigh;
		byte DataLenLow;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal unsafe struct SubQMediaCatalogNumber {
		SubQHeader header;
		byte FormatCode;
		byte Reserved0;
		byte Reserved1;
		byte Reserved2;
		internal byte ReservedMcVal;
		internal fixed byte MediaCatalog[15];
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal unsafe struct SubQTrackISRC {
		SubQHeader header;
		byte FormatCode;
		byte Reserved0;
		byte Track;
		byte Reserved1;
		internal byte ReservedTcVal;
		internal fixed byte TrackIsrc[15];
	}
}