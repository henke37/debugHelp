using System;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
	public class ModuleEntry {

		public UInt32 ProcessId;
		public IntPtr BaseAddress;
		public UInt32 Size;
		public IntPtr Handle;
		public string Name;
		public string Path;

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
		internal struct Native {
			internal UInt32 dwSize;
			UInt32 th32ModuleID;
			UInt32 th32ProcessID;
			UInt32 GlblcntUsage;
			UInt32 ProccntUsage;
			IntPtr modBaseAddr;
			UInt32 modBaseSize;
			IntPtr hModule;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_MODULE_NAME32 + 1)]
			string szModule;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
			string szExePath;

			internal ModuleEntry AsManaged() {
				return new ModuleEntry() {
					BaseAddress = modBaseAddr,
					Size = modBaseSize,
					Handle = hModule,
					ProcessId = th32ProcessID,
					Name = szModule,
					Path = szExePath
				};
			}

			private const int MAX_PATH = 260;
			private const int MAX_MODULE_NAME32=255;
		}
	}
}