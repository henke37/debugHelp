using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Memory {
	public class MemoryBasicInformation {
		public IntPtr BaseAddress;
		public IntPtr AllocationBase;
		public UInt32 AllocationProtect;
#if x86
		public UInt32 RegionSize;
#elif x64
		public UInt64 RegionSize;
#endif
		public MemoryPageState State;
		public MemoryProtection Protect;
		public MemoryBackingType Type;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct Native {
			IntPtr BaseAddress;
			IntPtr AllocationBase;
			UInt32 AllocationProtect;
#if x86
			internal UInt32 RegionSize;
#elif x64
			internal UInt64 RegionSize;
#endif
			UInt32 State;
			UInt32 Protect;
			UInt32 Type;

			public MemoryBasicInformation AsManaged() {
				return new MemoryBasicInformation() {
					BaseAddress=BaseAddress,
					AllocationBase=AllocationBase,
					AllocationProtect=AllocationProtect,
					RegionSize=RegionSize,
					State=(MemoryPageState)State,
					Protect=(MemoryProtection)Protect,
					Type=(MemoryBackingType)Type
				};
			}
		}

		public override string ToString() {
#if x86
			return $"0x{(uint)BaseAddress,8:X8} - 0x{(uint)BaseAddress+(uint)RegionSize,8:X8} {State} {Protect} {Type}";
#elif x64
			return $"0x{(uint)BaseAddress,16:X16} - 0x{(uint)BaseAddress+(uint)RegionSize,16:X16} {State} {Protect} {Type}";
#endif
		}
	}
}
