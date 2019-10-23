using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.RTTI.MSVC {
	public class ClassHierarchyDescriptor {
		public List<BaseClassDescriptor> BaseClasses;
		public ClassHierarchyFlags Flags;

		public ClassHierarchyDescriptor(List<BaseClassDescriptor> baseClasses, ClassHierarchyFlags flags) {
			BaseClasses = baseClasses ?? throw new ArgumentNullException(nameof(baseClasses));
			Flags = flags;
		}

		public BaseClassDescriptor? GetBaseClass(string mangledName) {
			foreach(var baseClass in BaseClasses) {
				if(baseClass.TypeDescriptor.MangledName == mangledName) return baseClass;
			}
			return null;
		}

		public BaseClassDescriptor? this[string mangledName] {
			get { return GetBaseClass(mangledName); }
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct MemoryStruct {
			internal uint Signature;
			internal ClassHierarchyFlags Flags;
			internal uint numBaseClasses;
			internal IntPtr pBaseClassArray;
		}

		[Flags]
		public enum ClassHierarchyFlags : uint {
			MultipleInhertience = 1,
			VirtualInhertience = 2
		}
	}
}
