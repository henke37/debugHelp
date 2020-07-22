using System;
using System.Linq;
using System.Collections.Generic;

using Henke37.Win32.AccessRights;
using Henke37.Win32.Files;
using Henke37.Win32.Memory;
using Henke37.Win32.Processes;
using Henke37.Win32.Snapshots;

namespace ModuleVsFileMapping {
	class Program {
		private string executableName;
		private readonly NativeFileNameConverter nameConverter;
		NativeProcess process;

		Dictionary<string, ModuleEntry> modules;

		public Program(string[] args) {
			executableName = args[0];
			nameConverter = new NativeFileNameConverter();
		}

		static void Main(string[] args) {
			new Program(args).Run();
		}

		private void Run() {
			using(var snap=new Toolhelp32Snapshot(Toolhelp32SnapshotFlags.Process)) {
				var entry = snap.GetProcesses().FirstOrDefault(p => p.Executable == executableName);
				process = entry.Open(ProcessAccessRights.QueryInformation | ProcessAccessRights.VMOperation | ProcessAccessRights.VMRead);
			}

			GatherModules();

			CheckMappedImages();
		}

		private void GatherModules() {
			modules = new Dictionary<string, ModuleEntry>();
			foreach(var module in process.GetModules()) {
				modules[module.Path.ToLowerInvariant()]=module;
			}
		}

		private void CheckMappedImages() {
			var ranges=process.QueryMemoryRangeInformation(UIntPtr.Zero, 0x7FFFFFFF);
			foreach(var range in ranges) {
				if(!range.Protect.IsExecutable()) continue;
				if(range.Type != MemoryBackingType.Private) {
					string backingFile = process.GetMappedFileName(range.BaseAddress);
					backingFile=nameConverter.NativeNameToDosName(backingFile).ToLowerInvariant();
					Console.WriteLine("{0,8:X} {1} {2}", (int)range.BaseAddress, range.Protect.ToString(), backingFile);
					if(!modules.ContainsKey(backingFile) && !modules.ContainsKey(Wow64Map(backingFile))) {

						Console.WriteLine("Unlisted!");
					}
				} else {
					Console.WriteLine("{0,8:X} {1}", (int)range.BaseAddress, range.Protect.ToString());
				}
			}
		}

		private string Wow64Map(string backingFile) {
			return backingFile.Replace(@"c:\windows\syswow64\", @"c:\windows\system32\");
		}
	}
}
