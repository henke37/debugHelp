﻿using Henke37.DebugHelp.Win32;
using System;
using System.Linq;
using Henke37.DebugHelp.Win32.AccessRights;

namespace ModuleVsFileMapping {
	class Program {
		private string executableName;
		private readonly NativeFileNameConverter nameConverter;
		NativeProcess process;

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

			GatherMappedImages();
		}

		private void GatherMappedImages() {
			var ranges=process.QueryMemoryRangeInformation(IntPtr.Zero, 0x0FFFFFFF);
			foreach(var range in ranges) {
				if(!range.Protect.IsExecutable()) continue;
				if(range.Type != MemoryBackingType.Private) {
					string backingFile = process.GetMappedFileName(range.BaseAddress);
					backingFile=nameConverter.NativeNameToDosName(backingFile);
					Console.WriteLine("{0:x} {1} {2}", range.BaseAddress, range.Protect.ToString(), backingFile);
				} else {
					Console.WriteLine("{0:x} {1}", range.BaseAddress, range.Protect.ToString());
				}
			}
		}
	}
}