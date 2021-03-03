using System;

namespace Henke37.Win32.Restart {
	public class ProcessInfo {
		UniqueProcess Process;
		string ApplicationName;
		string ServiceShortName;
		AppType ApplicationType;
		AppStatus AppStatus;
		UInt32 TSSessionId;
		bool Restartable;

		internal unsafe struct Native {
			UniqueProcess.Native Process;
			fixed char ApplicationName[256];
			fixed char ServiceShortName[64];
			AppType ApplicationType;
			AppStatus AppStatus;
			UInt32 TSSessionId;
			UInt32 Restartable;

			internal ProcessInfo AsNative() {
				string appNameN, svcNameN;
				fixed(char* appName=this.ApplicationName) {
					appNameN = new string(appName);
				}
				fixed(char* svcName = this.ServiceShortName) {
					svcNameN = new string(svcName);
				}
				return new ProcessInfo() {
					Process = Process.AsNative(),
					ApplicationName = appNameN,
					ServiceShortName = svcNameN,
					ApplicationType = ApplicationType,
					AppStatus = AppStatus,
					TSSessionId = TSSessionId,
					Restartable = Restartable != 0
				};
			}
		}
	}
}