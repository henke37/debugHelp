using System;
using System.Collections.Generic;

namespace Henke37.DebugHelp.RTTI.MSVC {
	public class TypeDescriptor {
		public string MangledName;

		public TypeDescriptor(string mangledName) {
			MangledName = mangledName;
		}

		public override string ToString() {
			return MangledName;
		}
	}
}
