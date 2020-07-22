using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.SafeHandles {
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal unsafe struct PublicObjectTypeInformation {
		UNICODE_STRING _TypeName;
		fixed UInt64 Reserved[22];

		public string TypeName {
			get => _TypeName.ToString();
		}
	}
}
