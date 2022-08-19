using System;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.RTTI.MSVC {
	public class BaseClassDescriptor {
		public TypeDescriptor TypeDescriptor;
		public uint NumContainedBases;
		public PMD DisplacementData;
		public BCDFlags Flags;
		public ClassHierarchyDescriptor? Hierarchy;

		public BaseClassDescriptor(TypeDescriptor typeDescriptor, uint numContainedBases, PMD displacementData, BCDFlags flags, ClassHierarchyDescriptor? hierarchy) {
			TypeDescriptor = typeDescriptor ?? throw new ArgumentNullException(nameof(typeDescriptor));
			NumContainedBases = numContainedBases;
			DisplacementData = displacementData;
			Flags = flags;
			Hierarchy = hierarchy;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct MemoryStruct {
			public IntPtr pTypeDescriptor;
			public uint NumContainedBases;
			public PMD DisplacementData;
			public BCDFlags Flags;
			public IntPtr pClassDescriptor;
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

		[Flags]
		public enum BCDFlags : UInt32 {
			NotVisible = 0x00000001,
			Ambiguous = 0x00000002,
			PrivateOrProtectedBase = 0x00000004,
			PrivateOrProtectedInCOMPOBJ = 0x00000008,
			VirtualBaseOfCONTOBJ = 0x00000010,
			Nonpolymorphic = 0x00000020,
			HasPCHD = 0x00000040
		}
	}
}
