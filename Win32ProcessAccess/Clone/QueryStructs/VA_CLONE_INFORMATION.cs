using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Clone.QueryStructs {
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	struct VA_CLONE_INFORMATION {
		public IntPtr Handle;
	}
}
