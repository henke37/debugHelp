using Henke37.DebugHelp;
using Henke37.Win32.AccessRights;
using Henke37.Win32.Files;
using Henke37.Win32.Memory;
using Henke37.Win32.Processes;
using Henke37.Win32.Snapshots;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ReadProcEnvVars {
	internal class Program {
		private string executableName;
		private NativeProcess process;
		private ProcessMemoryReader reader;

		static void Main(string[] args) {
			new Program(args);
		}
		public Program(string[] args) {
			executableName = args[0];

			using(var snap = new Toolhelp32Snapshot(Toolhelp32SnapshotFlags.Process)) {
				var entry = snap.GetProcesses().FirstOrDefault(p => p.Executable == executableName);
				process = entry.Open(ProcessAccessRights.QueryInformation | ProcessAccessRights.VMOperation | ProcessAccessRights.VMRead);
			}

			reader = new LiveProcessMemoryAccessor(process);

			var vars=ReadProcEnvVars();

            foreach (var envVar in vars) {
				Console.WriteLine(envVar);
            }
        }

		private List<string> ReadProcEnvVars() {
			IntPtr peb = process.PebBaseAddress;
			IntPtr procParamPtrAddr = peb + 0x10;
			IntPtr procParmPtr = reader.ReadIntPtr(procParamPtrAddr);

			IntPtr envDataPtr = reader.ReadIntPtr(procParmPtr + 0x48);
			UInt32 envSize = reader.ReadUInt32(procParmPtr + 0x0290);

			var bytes = reader.ReadBytes(envDataPtr, envSize);

			List<string> vars = new List<string>();

			using(var r = new BinaryReader(new MemoryStream(bytes), Encoding.Unicode)) {
				var chars = r.ReadChars(bytes.Length / 2);

				int varStart = 0;
				for(int charIndex=0;charIndex < chars.Length;charIndex++) {
					char c = chars[charIndex];
					if(c==0) {
						int varLen = charIndex - varStart;
						string var = new string(chars, varStart, varLen);
						vars.Add(var);
						varStart = charIndex + 1;
					}
				}
			}

			return vars;
		}
	}
}
