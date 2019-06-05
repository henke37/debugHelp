using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.RTTI.MSVC {
	internal class CompleteObjectLocator {
		public uint ObjectRootOffset;
		public uint ConstructorDescriptorOffset;
		public TypeDescriptor TypeDescriptor;
		public ClassHierarchyDescriptor ClassHierarchyDescriptor;

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct MemoryStruct {
			public uint Signature;
			public uint ObjectRootOffset;
			public uint ConstructorDescriptorOffset;
			public uint pTypeDescriptor;
			public uint pClassDescriptor;
		}

		public uint LocateCompleteObject(uint objAddr) {
			return objAddr - ObjectRootOffset;
		}
	}
}
