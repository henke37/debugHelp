using Henke37.DebugHelp;
using Henke37.DebugHelp.PdbAccess;
using Henke37.Win32.AccessRights;
using Henke37.Win32.Memory;
using Henke37.Win32.Processes;
using Henke37.Win32.Snapshots;
using Henke37.Win32.Threads;

using Stackwalker;
using System;
using System.Linq;

namespace StackDumper {
	class Program {
		private string executableName;

		private NativeProcess process;
		private SymbolResolver resolver;
		private ProcessMemoryAccessor memoryReader;
		private StackWalker walker;

		static void Main(string[] args) {
			var p=new Program(args);
			p.DumpStacks();
		}

		private void DumpStacks() {
			using(var snap = new Toolhelp32Snapshot(Toolhelp32SnapshotFlags.Thread)) {
				foreach(var threadEntry in snap.GetThreads(process.ProcessId)) {
					Console.WriteLine($"Thread # {threadEntry.ThreadId}");
					DumpThread(threadEntry);
				}
			}
		}

		private void DumpThread(ThreadEntry threadEntry) {
			using(NativeThread thread=threadEntry.Open(ThreadAcccessRights.GetContext|ThreadAcccessRights.SuspendResume|ThreadAcccessRights.QueryInformation)) {
				walker = new StackWalker(thread, memoryReader, resolver);

				try {
					thread.Suspend();
					var stack=walker.Walk();
					foreach(var frame in stack) {
						var fun = resolver.FindFunctionAtAddr((IntPtr)frame.returnAddress);
						Console.WriteLine($"{fun.name} {fun.virtualAddress-frame.returnAddress}");
					}
				} finally {
					thread.Resume();
				}
			}
		}

		private Program(string[] args) {
			executableName = args[0];

			using(var snap = new Toolhelp32Snapshot(Toolhelp32SnapshotFlags.Process)) {
				var procEntry = snap.GetProcesses().FirstOrDefault(p => p.Executable == executableName);
				
				process = procEntry.Open(ProcessAccessRights.VMOperation | ProcessAccessRights.VMRead | ProcessAccessRights.Synchronize | ProcessAccessRights.QueryInformation);
			}
			ModuleEntry mainModule = process.GetModules().First(m => m.Name == executableName);

			string pdbPath = mainModule.Path.Replace(".exe", ".pdb");
			resolver = new SymbolResolver();
			resolver.AddPdb(pdbPath,mainModule.BaseAddress);
			memoryReader = new LiveProcessMemoryAccessor(process);
		}
	}
}
