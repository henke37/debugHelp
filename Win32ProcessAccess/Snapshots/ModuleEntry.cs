using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Snapshots {
	public class ModuleEntry {

		public UInt32 ProcessId;
		public IntPtr BaseAddress;
		public UInt32 Size;
		public IntPtr Handle;
		public string Name;
		public string Path;

		public ModuleEntry(IntPtr baseAddress, uint size, IntPtr handle, uint processId, string name, string path) {
			BaseAddress = baseAddress;
			Size = size;
			Handle = handle;
			ProcessId = processId;
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Path = path ?? throw new ArgumentNullException(nameof(path));
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
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
				return new ModuleEntry(modBaseAddr, modBaseSize, hModule, th32ProcessID, szModule, szExePath);
			}

			private const int MAX_PATH = 260;
			private const int MAX_MODULE_NAME32 = 255;
		}

		public override string ToString() {
			return $"{Name} 0x{BaseAddress:x}";
		}
	}
}