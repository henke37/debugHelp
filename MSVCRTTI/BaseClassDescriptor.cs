using System;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.RTTI.MSVC {
	public class BaseClassDescriptor {
		public TypeDescriptor TypeDescriptor;
		public uint NumContainedBases;
		public PMD DisplacementData;

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct MemoryStruct {
			public IntPtr pTypeDescriptor;
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

			public IntPtr LocateBaseObject(IntPtr completeObjectAddr, ProcessMemoryReader reader) {
				completeObjectAddr += mdisp;
				if(pdisp != -1) {
					IntPtr vtbl = completeObjectAddr + pdisp;
					completeObjectAddr += reader.ReadInt32(vtbl + vdisp);
				}
				return completeObjectAddr;
			}
		}
	}
}
