using Henke37.Win32.Jobs;
using Henke37.Win32.Processes;
using Henke37.Win32.Snapshots;
using Henke37.Win32.AccessRights;
using System;
using System.Linq;

namespace JobTest {
	class Program {
		static void Main(string[] args) {
			using(NativeJob job = NativeJob.Create()) {
				var extended = job.ExtendedLimitInformation;
				extended.LimitFlags = LimitFlags.JobTime | LimitFlags.JobMemory;
				extended.PerJobUserTimeLimit = new TimeSpan(0, 5, 0);
				extended.JobMemoryLimit = 1024 * 1024 * 100;
				job.ExtendedLimitInformation = extended;

				if(args.Length >= 1) {

					using(var atts = new ProcThreadAttributeList(1)) {

						var shellProc = GetShellProcess();

						using(NativeProcess natProcess = NativeProcess.CreateProcess(args[0], null, CreateProcessFlags.None, StartupInfoFlags.None, atts, null, out _)) {
							job.AttachProcess(natProcess);
						}
					}
				}
			}
			
		}

		static NativeProcess GetShellProcess() {
			using(var snap = new Toolhelp32Snapshot(Toolhelp32SnapshotFlags.Process)) {
				return snap.GetProcesses().Where(p => p.Executable == "explorer.exe").First().Open(ProcessAccessRights.All);
			}
		}
	}
}
