using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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

			CompleteObjectLocator locator = new CompleteObjectLocator(
				GetTypeDescriptor(memoryStruct.pTypeDescriptor),
				GetClassHierarchyDescriptor(memoryStruct.pClassDescriptor),
				memoryStruct.ObjectRootOffset,
				memoryStruct.ConstructorDescriptorOffset
			);

			return locator;
		}

		private ClassHierarchyDescriptor GetClassHierarchyDescriptor(IntPtr pClassDescriptor) {
			ClassHierarchyDescriptor desc;
			if(classDescriptorMap.TryGetValue(pClassDescriptor, out desc)) return desc;
			return ReadClassHierarchyDescriptor(pClassDescriptor);
		}

		private ClassHierarchyDescriptor ReadClassHierarchyDescriptor(IntPtr pClassDescriptor) {
			ClassHierarchyDescriptor.MemoryStruct memoryStruct = new ClassHierarchyDescriptor.MemoryStruct();
			processMemoryReader.ReadStruct(pClassDescriptor, ref memoryStruct);

			if(memoryStruct.Signature != 0) throw new InvalidDataException("Invalid class hierarchy signature");

			ClassHierarchyDescriptor desc = new ClassHierarchyDescriptor(
				new List<BaseClassDescriptor>((int)memoryStruct.numBaseClasses),
				memoryStruct.Flags
			);

			classDescriptorMap[pClassDescriptor] = desc;

			IntPtr[] baseDescriptorPointers = processMemoryReader.ReadIntPtrArray(memoryStruct.pBaseClassArray, memoryStruct.numBaseClasses);

			for(uint baseClassIndex = 0; baseClassIndex < memoryStruct.numBaseClasses; ++baseClassIndex) {
				desc.BaseClasses.Add(ReadBaseClassDescriptor(baseDescriptorPointers[baseClassIndex]));
			};

			return desc;
		}

		private BaseClassDescriptor ReadBaseClassDescriptor(IntPtr baseClassDescriptorPointer) {
			BaseClassDescriptor.MemoryStruct memoryStruct = new BaseClassDescriptor.MemoryStruct();
			processMemoryReader.ReadStruct(baseClassDescriptorPointer, ref memoryStruct);

			ClassHierarchyDescriptor hierarchy = null;
			if((memoryStruct.Flags & BaseClassDescriptor.BCDFlags.HasPCHD) != 0) {
				hierarchy = GetClassHierarchyDescriptor(memoryStruct.pClassDescriptor);
			}

			BaseClassDescriptor desc = new BaseClassDescriptor(
				GetTypeDescriptor(memoryStruct.pTypeDescriptor),
				memoryStruct.NumContainedBases,
				memoryStruct.DisplacementData,
				memoryStruct.Flags,
				hierarchy
			);
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

		public ClassHierarchyDescriptor GetMangledHeirarchyFromObjPtr(IntPtr objAddr) {
			var col = readObjPtr(objAddr);
			return col.ClassHierarchyDescriptor;
		}

		public IntPtr DynamicCast(IntPtr objAddr, string mangledBaseClassName) {
			var col = readObjPtr(objAddr);
			var baseClass = GetBaseClass(col, mangledBaseClassName);
			if(baseClass == null) throw new InvalidCastException(string.Format(Resources.BadDynamicCast_NoSuchBase, mangledBaseClassName));

			IntPtr completeObjAddr = col.LocateCompleteObject(objAddr, processMemoryReader);
			return baseClass.DisplacementData.LocateBaseObject(completeObjAddr, processMemoryReader);
		}

		public bool TryDynamicCast(IntPtr objAddr, string mangledBaseClassName, out IntPtr resAddr) {
			var col = readObjPtr(objAddr);
			var baseClass = GetBaseClass(col, mangledBaseClassName);
			if(baseClass == null) { resAddr = IntPtr.Zero; return false; }

			IntPtr completeObjAddr = col.LocateCompleteObject(objAddr, processMemoryReader);
			resAddr=baseClass.DisplacementData.LocateBaseObject(completeObjAddr, processMemoryReader);
			return true;
		}

		private BaseClassDescriptor? GetBaseClass(CompleteObjectLocator col, string mangledName) {
			BaseClassDescriptor? found = null;
			foreach(var baseClass in col.ClassHierarchyDescriptor.BaseClasses) {
				if(baseClass.TypeDescriptor.MangledName == mangledName) {
					if(found != null) throw new AmbiguousMatchException("Base class is ambigious!");
					found = baseClass;
				}
			}
			return found;
		}
	}
}
