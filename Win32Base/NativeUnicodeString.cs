using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Henke37.Win32.Base {
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal unsafe struct NativeUnicodeString {

		//In BYTES
		UInt16 Length;
		UInt16 MaxLength;

		//Char!=byte in C#
		char* Buffer;

		public string AsManagedString() {
			return new string(Buffer);
		}
	}
}
