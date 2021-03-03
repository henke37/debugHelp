using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.Restart {
	public class RestartManager : IDisposable {

		private UIntPtr handle;
		private bool disposedValue;

		public RestartManager(out string sessionKey) {
			var sb = new StringBuilder();
			sb.Capacity = 128;
			var result = RmStartSession(out handle, 0, sb);
			sessionKey = sb.ToString();
			CheckResult(result);
		}

		private RestartManager(UIntPtr handle) { this.handle = handle; }

		public static RestartManager JoinSession(string sessionKey) {
			var result = RmJoinSession(out var handle, sessionKey);

			CheckResult(result);
			return new RestartManager(handle);
		}

		private static void CheckResult(RMResult result) {
			if(result != RMResult.Success) throw new Exception();
		}

		[DllImport("Rstrtmgr.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern RMResult RmStartSession(out UIntPtr handle, UInt32 flags, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder sessionKey);

		[DllImport("Rstrtmgr.dll", ExactSpelling = true, SetLastError = true)]
		private static extern RMResult RmJoinSession(out UIntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string sessionKey);

		[DllImport("Rstrtmgr.dll", ExactSpelling = true, SetLastError = true)]
		private static extern RMResult RmEndSession(UIntPtr handle);

		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					// TODO: dispose managed state (managed objects)
				}

				RmEndSession(handle);
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		~RestartManager() {
		    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		     Dispose(disposing: false);
		}

		public void Dispose() {
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
