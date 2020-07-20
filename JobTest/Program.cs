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
				var basic = job.BasicLimitInformation;

				basic.LimitFlags = LimitFlags.JobTime;
				basic.PerJobUserTimeLimit = new TimeSpan(0, 5, 0);
				job.BasicLimitInformation = basic;
			}
			
		}
	}
}
