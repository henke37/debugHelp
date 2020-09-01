using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.Processes {
	internal unsafe struct StartupInfoExW {
		StartupInfoW StartupInfo;
		ProcThreadAttributeList.Native *attributeList;

		internal StartupInfoExW(StartupInfoW StartupInfo, ProcThreadAttributeList atts) {
			this.StartupInfo = StartupInfo;
			attributeList = atts.lpAttributeList;
		}
	}
}
