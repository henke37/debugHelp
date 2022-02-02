using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Clone.QueryStructs {
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	class HANDLE_TRACE_INFORMATION {
		public IntPtr SectionHandle;
		public IntPtr SectionSize;
	}
}
