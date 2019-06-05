using System;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.RTTI.MSVC {
	public class BaseClassDescriptor {
		public TypeDescriptor TypeDescriptor;
		public uint NumContainedBases;
		public PMD DisplacementData;

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct MemoryStruct {
			public uint pTypeDescriptor;
			public uint NumContainedBases;
			public PMD DisplacementData;
			public uint Flags;
		}

		public override string ToString() {
			return TypeDescriptor.MangledName;
		}

		public struct PMD {
#pragma warning disable CS0649
			int mdisp;
			int pdisp;
			int vdisp;
#pragma warning restore CS0649

			public uint LocateBaseObject(uint completeObjectAddr, ProcessMemoryReader reader) {
				completeObjectAddr += (uint)mdisp;
				if(pdisp != -1) {
					uint vtbl = (uint)(completeObjectAddr + pdisp);
					completeObjectAddr += reader.ReadUInt32((uint)(vtbl + vdisp));
				}
				return completeObjectAddr;
			}
		}
	}
}
