using Henke37.Win32.Processes;
using Henke37.Win32.Tokens;
using System.Security.Principal;

namespace TokenTest {
	class Program {
		static void Main(string[] args) {
			using(NativeToken token=NativeProcess.Current.OpenToken(TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges)) {
				var privs = token.Privileges;
				token.AdjustPrivilege(Privilege.IncreaseWorkingSet, PrivilegeAttributes.Enabled, out _);

				var linked = token.GetLinkedToken();
			}
		}
	}
}
