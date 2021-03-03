using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.Restart {
	public class RestartManager : IDisposable {

		private RestartManagerHandle handle;

		public RestartManager(string sessionKey) {
			var result = RmStartSession(out handle, 0, sessionKey);
			CheckResult(result);
		}

		private RestartManager(RestartManagerHandle handle) { this.handle = handle; }

		public static RestartManager JoinSession(string sessionKey) {
			var result = RmJoinSession(out var handle, sessionKey);
			CheckResult(result);
			return new RestartManager(handle);
		}

		private static void CheckResult(RMResult result) {
			if(result != RMResult.Success) throw new Exception();
		}

		public void Dispose() {
			((IDisposable)handle).Dispose();
		}

		[DllImport("Rstrtmgr.dll", ExactSpelling = true, SetLastError = true)]
		private static extern RMResult RmStartSession(out RestartManagerHandle handle, UInt32 flags, [MarshalAs(UnmanagedType.LPWStr)] string sessionKey);

		[DllImport("Rstrtmgr.dll", ExactSpelling = true, SetLastError = true)]
		private static extern RMResult RmJoinSession(out RestartManagerHandle handle, [MarshalAs(UnmanagedType.LPWStr)] string sessionKey);
	}
}
