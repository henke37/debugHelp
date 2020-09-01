using Henke37.Win32.Jobs;
using Henke37.Win32.Processes;
using System;

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
					using(NativeProcess natProcess = NativeProcess.CreateProcess(args[0],null, CreateProcessFlags.None, StartupInfoFlags.None, null, out _)) {
						job.AttachProcess(natProcess);
					}
				}
			}
			
		}
	}
}
