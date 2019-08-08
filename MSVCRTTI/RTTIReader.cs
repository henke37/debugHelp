using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Henke37.DebugHelp.RTTI.MSVC {
	public class RTTIReader {
		private ProcessMemoryReader processMemoryReader;

		private Dictionary<IntPtr, CompleteObjectLocator> completeObjectLocatorMap;
		private Dictionary<IntPtr, TypeDescriptor> typeDescriptorMap;
		private Dictionary<IntPtr, ClassHierarchyDescriptor> classDescriptorMap;

		public RTTIReader(ProcessMemoryReader processMemoryReader) {
			this.processMemoryReader = processMemoryReader ?? throw new ArgumentNullException(nameof(processMemoryReader));

			completeObjectLocatorMap = new Dictionary<IntPtr, CompleteObjectLocator>();
			typeDescriptorMap = new Dictionary<IntPtr, TypeDescriptor>();
			classDescriptorMap = new Dictionary<IntPtr, ClassHierarchyDescriptor>();
		}

		private CompleteObjectLocator readObjPtr(IntPtr objAddr) {
			IntPtr vtblPtrVal = processMemoryReader.ReadIntPtr(objAddr);
			IntPtr metaPtrVal = processMemoryReader.ReadIntPtr(vtblPtrVal - 4);

			if(completeObjectLocatorMap.TryGetValue(metaPtrVal, out CompleteObjectLocator objectLocator)) {
				return objectLocator;
			}

			var col = ReadCol(metaPtrVal);
			completeObjectLocatorMap[metaPtrVal] = col;

			return col;
		}

		private CompleteObjectLocator ReadCol(IntPtr metaPtrVal) {
			CompleteObjectLocator.MemoryStruct memoryStruct = new CompleteObjectLocator.MemoryStruct();
			processMemoryReader.ReadStruct(metaPtrVal, ref memoryStruct);

			if(memoryStruct.Signature != 0) throw new InvalidDataException("Invalid COL signature");

			Debug.Assert(memoryStruct.pTypeDescriptor != IntPtr.Zero, "pTypeDescriptor shouldn't be 0!");
			Debug.Assert(memoryStruct.pClassDescriptor != IntPtr.Zero, "pClassDescriptor shouldn't be 0!");

			CompleteObjectLocator locator = new CompleteObjectLocator();
			locator.ConstructorDescriptorOffset = memoryStruct.ConstructorDescriptorOffset;
			locator.ObjectRootOffset = memoryStruct.ObjectRootOffset;
			locator.TypeDescriptor = GetTypeDescriptor(memoryStruct.pTypeDescriptor);
			locator.ClassHierarchyDescriptor = GetClassHierarchyDescriptor(memoryStruct.pClassDescriptor);

			return locator;
		}

		private ClassHierarchyDescriptor GetClassHierarchyDescriptor(IntPtr pClassDescriptor) {
			ClassHierarchyDescriptor desc;
			if(classDescriptorMap.TryGetValue(pClassDescriptor, out desc)) return desc;
			desc = ReadClassHierarchyDescriptor(pClassDescriptor);
			classDescriptorMap[pClassDescriptor] = desc;
			return desc;
		}

		private ClassHierarchyDescriptor ReadClassHierarchyDescriptor(IntPtr pClassDescriptor) {
			ClassHierarchyDescriptor.MemoryStruct memoryStruct = new ClassHierarchyDescriptor.MemoryStruct();
			processMemoryReader.ReadStruct(pClassDescriptor, ref memoryStruct);

			if(memoryStruct.Signature != 0) throw new InvalidDataException("Invalid class hierarchy signature");

			ClassHierarchyDescriptor desc = new ClassHierarchyDescriptor();
			desc.Flags = memoryStruct.Flags;
			desc.BaseClasses = new List<BaseClassDescriptor>((int)memoryStruct.numBaseClasses);

			IntPtr[] baseDescriptorPointers =processMemoryReader.ReadIntPtrArray(memoryStruct.pBaseClassArray, memoryStruct.numBaseClasses);

			for(uint baseClassIndex = 0; baseClassIndex < memoryStruct.numBaseClasses; ++baseClassIndex) {
				desc.BaseClasses.Add(ReadBaseClassDescriptor(baseDescriptorPointers[baseClassIndex]));
			};

			return desc;
		}

		private BaseClassDescriptor ReadBaseClassDescriptor(IntPtr baseClassDescriptorPointer) {
			BaseClassDescriptor.MemoryStruct memoryStruct = new BaseClassDescriptor.MemoryStruct();
			processMemoryReader.ReadStruct(baseClassDescriptorPointer, ref memoryStruct);

			BaseClassDescriptor desc = new BaseClassDescriptor();
			desc.TypeDescriptor = GetTypeDescriptor(memoryStruct.pTypeDescriptor);
			desc.DisplacementData = memoryStruct.DisplacementData;
			desc.NumContainedBases = memoryStruct.NumContainedBases;
			return desc;
		}

		private TypeDescriptor GetTypeDescriptor(IntPtr pTypeDescriptor) {
			TypeDescriptor desc;
			if(typeDescriptorMap.TryGetValue(pTypeDescriptor, out desc)) return desc;
			desc = ReadTypeDescriptor(pTypeDescriptor);
			typeDescriptorMap[pTypeDescriptor] = desc;
			return desc;
		}

		private TypeDescriptor ReadTypeDescriptor(IntPtr pTypeDescriptor) {
			return new TypeDescriptor(
				processMemoryReader.ReadNullTermString(pTypeDescriptor + 2 * 4)
			);
		}

		public string GetMangledClassNameFromObjPtr(IntPtr objAddr) {
			var col = readObjPtr(objAddr);
			return col.TypeDescriptor.MangledName;
		}

		public BaseClassDescriptor GetBaseClassDescriptorForObject(IntPtr objAddr, string mangledName) {
			var col = readObjPtr(objAddr);
			return col.ClassHierarchyDescriptor[mangledName];
		}

		public ClassHierarchyDescriptor GetMangledHeirarchyFromObjPtr(IntPtr objAddr) {
			var col = readObjPtr(objAddr);
			return col.ClassHierarchyDescriptor;
		}

		public IntPtr DynamicCast(IntPtr objAddr, string mangledBaseClassName) {
			var col = readObjPtr(objAddr);
			var baseClass = col.ClassHierarchyDescriptor[mangledBaseClassName];
			if(baseClass == null) throw new InvalidCastException(string.Format(Resources.BadDynamicCast_NoSuchBase, mangledBaseClassName));

			IntPtr completeObjAddr = col.LocateCompleteObject(objAddr);
			return baseClass.DisplacementData.LocateBaseObject(completeObjAddr, processMemoryReader);
		}

		public bool TryDynamicCast(IntPtr objAddr, string mangledBaseClassName, out IntPtr resAddr) {
			var col = readObjPtr(objAddr);
			var baseClass = col.ClassHierarchyDescriptor[mangledBaseClassName];
			if(baseClass == null) { resAddr = IntPtr.Zero; return false; }

			IntPtr completeObjAddr = col.LocateCompleteObject(objAddr);
			resAddr=baseClass.DisplacementData.LocateBaseObject(completeObjAddr, processMemoryReader);
			return true;
		}

		public bool HasBaseClass(IntPtr objAddr, string mangledBaseClassName) {
			var col = readObjPtr(objAddr);
			return null != col.ClassHierarchyDescriptor[mangledBaseClassName];
		}
	}
}
