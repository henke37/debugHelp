using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Clone.QueryStructs {
	class VASpaceEntry {
		public IntPtr BaseAddress;
		public IntPtr AllocationBase;
		public UInt32 AllocationProtect;
		public IntPtr RegionSize;
		public UInt32 State;
		public UInt32 Protect;
		public UInt32 Type;
		public UInt32 TimeDateStamp;
		public UInt32 SizeOfImage;
		public IntPtr ImageBase;
		public UInt32 CheckSum;
		public String MappedFileName;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal unsafe struct Native {

			IntPtr BaseAddress;
			IntPtr AllocationBase;
			UInt32 AllocationProtect;
			IntPtr RegionSize;
			UInt32 State;
			UInt32 Protect;
			UInt32 Type;
			UInt32 TimeDateStamp;
			UInt32 SizeOfImage;
			IntPtr ImageBase;
			UInt32 CheckSum;
			UInt16 MappedFileNameLength;
			char* MappedFileName;

			internal VASpaceEntry AsManaged() {
				return new VASpaceEntry() {
					BaseAddress = BaseAddress,
					AllocationBase = AllocationBase,
					AllocationProtect = AllocationProtect,
					RegionSize = RegionSize,
					State = State,
					Protect = Protect,
					Type = Type,
					TimeDateStamp = TimeDateStamp,
					SizeOfImage = SizeOfImage,
					ImageBase = ImageBase,
					CheckSum = CheckSum
				};
			}
		}
	}
}
