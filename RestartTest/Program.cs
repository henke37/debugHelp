using Henke37.Win32.Restart;

namespace RestartTest {
	class Program {
		static void Main(string[] args) {
			using(RestartManager mng = new RestartManager(out _)) {
				;
			}
		}
	}
}
