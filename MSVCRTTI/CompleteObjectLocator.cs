using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.RTTI.MSVC {
	internal class CompleteObjectLocator {
		public int ObjectRootOffset;
		public int ConstructorDescriptorOffset;
		public TypeDescriptor TypeDescriptor;
		public ClassHierarchyDescriptor ClassHierarchyDescriptor;

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct MemoryStruct {
			public uint Signature;
			public int ObjectRootOffset;
			public int ConstructorDescriptorOffset;
			public IntPtr pTypeDescriptor;
			public IntPtr pClassDescriptor;
		}

		public IntPtr LocateCompleteObject(IntPtr objAddr) {
			return objAddr - ObjectRootOffset;
		}
	}
}
