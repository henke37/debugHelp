using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Clone.QueryStructs {
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	struct THREAD_INFORMATION {
		public UInt32 ThreadsCaptured;
		public UInt32 ContextSize;
	}
}
