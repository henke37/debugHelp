using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.RTTI.MSVC {
	internal class CompleteObjectLocator {
		public TypeDescriptor TypeDescriptor;
		public ClassHierarchyDescriptor ClassHierarchyDescriptor;
		public int ObjectRootOffset;
		public int ConstructorDescriptorOffset;

		public CompleteObjectLocator(TypeDescriptor typeDescriptor, ClassHierarchyDescriptor classHierarchyDescriptor, int objectRootOffset, int constructorDescriptorOffset) {
			TypeDescriptor = typeDescriptor ?? throw new ArgumentNullException(nameof(typeDescriptor));
			ClassHierarchyDescriptor = classHierarchyDescriptor ?? throw new ArgumentNullException(nameof(classHierarchyDescriptor));
			ObjectRootOffset = objectRootOffset;
			ConstructorDescriptorOffset = constructorDescriptorOffset;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct MemoryStruct {
			public uint Signature;
			public int ObjectRootOffset;
			public int ConstructorDescriptorOffset;
			public IntPtr pTypeDescriptor;
			public IntPtr pClassDescriptor;
		}

		public IntPtr LocateCompleteObject(IntPtr objAddr, ProcessMemoryReader processMemoryReader) {
			var completeAddr = objAddr - ObjectRootOffset;

			if(ConstructorDescriptorOffset !=0 ) {
				int offsetVal = processMemoryReader.ReadInt32(objAddr - ConstructorDescriptorOffset);
				completeAddr -= offsetVal;
			}

			return completeAddr;
		}
	}
}
