using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Base.SafeHandles {
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal unsafe struct PublicObjectTypeInformation {
		NativeUnicodeString _TypeName;
		fixed UInt32 Reserved[22];

		public string TypeName {
			get => _TypeName.AsManagedString();
		}
	}
}
