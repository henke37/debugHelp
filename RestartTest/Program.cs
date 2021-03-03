using Henke37.Win32.Restart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestartTest {
	class Program {
		static void Main(string[] args) {
			using(RestartManager mng = new RestartManager("test")) {
				;
			}
		}
	}
}
