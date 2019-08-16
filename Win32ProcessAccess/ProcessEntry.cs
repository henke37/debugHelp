using System;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
	public class ProcessEntry {
		public UInt32 ProcessId;
		public UInt32 ThreadCount;
		public UInt32 ParentProcessId;
		public string Executable;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct Native {
			internal UInt32 dwSize;
			UInt32 cntUsage;
			UInt32 th32ProcessID;
			IntPtr th32DefaultHeapID;
			UInt32 th32ModuleID;
			UInt32 cntThreads;
			UInt32 th32ParentProcessID;
			Int32 pcPriClassBase;
			UInt32 dwFlags;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
			string szExePath;

			private const int MAX_PATH = 260;

			internal ProcessEntry AsManaged() {
				return new ProcessEntry() {
					Executable = szExePath,
					ProcessId = th32ProcessID, ParentProcessId = th32ParentProcessID,
					ThreadCount = cntThreads
				};
			}
		}
	}
}