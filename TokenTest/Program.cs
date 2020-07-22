using Henke37.Win32.Processes;
using Henke37.Win32.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenTest {
	class Program {
		static void Main(string[] args) {
			using(NativeToken token=NativeProcess.Current.OpenToken(System.Security.Principal.TokenAccessLevels.Query)) {
				var privs = token.Privileges;
			}
		}
	}
}
