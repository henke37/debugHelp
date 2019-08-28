namespace Henke37.DebugHelp.Win32 {
	public enum FileMappingFlags : uint {
		Commit = 0x8000000,
		Image = 0x1000000,
		ImageNoExecute = 0x11000000,
		LargePages = 0x80000000,
		NoCache = 0x10000000,
		Reserve = 0x4000000,
		WriteCombine = 0x40000000
	}
}