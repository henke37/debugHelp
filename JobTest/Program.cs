using Henke37.DebugHelp.Win32;
using Henke37.DebugHelp.Win32.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTest {
	class Program {
		static void Main(string[] args) {

			using(NativeJob job = NativeJob.Create()) {
				var extended = job.ExtendedLimitInformation;

				extended.LimitFlags = LimitFlags.JobTime | LimitFlags.JobMemory;
				extended.PerJobUserTimeLimit = new TimeSpan(0, 5, 0);
				extended.JobMemoryLimit = 1024 * 1024 * 100;
				job.ExtendedLimitInformation = extended;
			}
			
		}
	}
}
