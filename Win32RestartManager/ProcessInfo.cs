﻿using System;

namespace Henke37.Win32.Restart {
	public class ProcessInfo {
		public UniqueProcess Process;
		public string ApplicationName;
		public string ServiceShortName;
		public AppType ApplicationType;
		public AppStatus AppStatus;
		public UInt32 TSSessionId;
		public bool Restartable;

		internal unsafe struct Native {
			UniqueProcess.Native Process;
			fixed sbyte ApplicationName[256*2];
			fixed sbyte ServiceShortName[64*2];
			AppType ApplicationType;
			AppStatus AppStatus;
			UInt32 TSSessionId;
			UInt32 Restartable;

			internal ProcessInfo AsNative() {
				string appNameN, svcNameN;
				fixed(sbyte* appName=this.ApplicationName) {
					appNameN = new string((char*)appName);
				}
				fixed(sbyte* svcName = this.ServiceShortName) {
					svcNameN = new string((char*)svcName);
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

		public override string ToString() {
			return $"{Process.ProcessId} {ApplicationName}";
		}
	}
}