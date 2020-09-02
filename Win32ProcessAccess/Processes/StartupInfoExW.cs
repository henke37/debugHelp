namespace Henke37.Win32.Processes {
	internal unsafe struct StartupInfoExW {
		public StartupInfoW StartupInfo;
		public ProcThreadAttributeList.Native *attributeList;

		internal StartupInfoExW(StartupInfoW StartupInfo, ProcThreadAttributeList atts) {
			this.StartupInfo = StartupInfo;
			this.StartupInfo.cb = (uint)sizeof(StartupInfoExW);
			attributeList = atts.lpAttributeList;
		}
	}
}
