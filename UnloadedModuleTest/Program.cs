using Henke37.Win32.Memory;
using Henke37.Win32.Processes;
using Henke37.Win32.LastUnloadedModules;

namespace UnloadedModuleTest {
	class Program {
		static void Main(string[] args) {
			var proc = NativeProcess.Open(24032);
			var procMem = new LiveProcessMemoryAccessor(proc);
			var unloaded = new UnloadedModulesAccessor(procMem);

			var modules = unloaded.ReadUnloadedModules();
		}
	}
}
