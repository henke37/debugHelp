using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.Restart {
	class RestartManagerHandle : SafeHandleZeroOrMinusOneIsInvalid {
		public RestartManagerHandle(bool ownsHandle) : base(ownsHandle) {
		}

		protected override bool ReleaseHandle() {
			RMResult result = RmEndSession(handle);
			return result == 0;
		}

		[DllImport("Rstrtmgr.dll", ExactSpelling = true, SetLastError = true)]
		public static extern RMResult RmEndSession(IntPtr handle);
	}
}

