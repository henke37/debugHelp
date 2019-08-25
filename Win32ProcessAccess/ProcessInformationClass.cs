namespace Henke37.DebugHelp.Win32 {
	internal enum ProcessInformationClass {
		BasicInformation = 0,
		DebugPort = 7,
		Wow64Information = 26,
		ImageFileName = 27,
		BreakOnTermination = 29,
		SubsystemInformation = 75
	}
}