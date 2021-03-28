using Henke37.Win32.AccessRights;
using Henke37.Win32.Processes;
using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Snapshots {
	public class ProcessEntry {
		public UInt32 ProcessId;
		public UInt32 ThreadCount;
		public UInt32 ParentProcessId;
		public string Executable;

		public ProcessEntry(uint processId, uint parentProcessId, string executable, uint threadCount) {
			ProcessId = processId;
			ThreadCount = threadCount;
			ParentProcessId = parentProcessId;
			Executable = executable ?? throw new ArgumentNullException(nameof(executable));
		}

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
				return new ProcessEntry(th32ProcessID, th32ParentProcessID, szExePath, cntThreads);
			}
		}

		public NativeProcess Open(ProcessAccessRights rights = ProcessAccessRights.All, bool inheritable = false) {
			return NativeProcess.Open(ProcessId, rights, false);
		}

		public override string ToString() {
			return $"{ProcessId}: \"{Executable}\"";
		}

		public void Deconstruct(
			out UInt32 ProcessId,
			out UInt32 ThreadCount,
			out UInt32 ParentProcessId,
			out string Executable
		) {
			ProcessId = this.ProcessId;
			ThreadCount = this.ThreadCount;
			ParentProcessId = this.ParentProcessId;
			Executable = this.Executable;
		}

		public void Deconstruct(
			out UInt32 ProcessId,
			out string Executable
		) {
			ProcessId = this.ProcessId;
			Executable = this.Executable;
		}
	}
}