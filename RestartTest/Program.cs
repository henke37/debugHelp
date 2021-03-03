using Henke37.Win32.Restart;

namespace RestartTest {
	class Program {
		static void Main(string[] args) {
			using(RestartManager mng = new RestartManager(out _)) {
				string[] fileNames = new string[1];
				fileNames[0] = @"c:\windows\system32\calc.exe";
				mng.RegisterResources(fileNames, null);
			}
		}
	}
}
