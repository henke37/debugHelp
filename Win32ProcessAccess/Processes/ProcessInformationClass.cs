using Henke37.Win32.Base;

namespace Henke37.Win32.Processes {
	[Undocumented]
	internal enum ProcessInformationClass {
		BasicInformation = 0,
		DebugPort = 7,
		Wow64Information = 26,
		ImageFileName = 27,
		BreakOnTermination = 29,
		HandleInformation = 51,
		SubsystemInformation = 75
	}
}