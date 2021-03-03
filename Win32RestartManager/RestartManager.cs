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

		public void CancelCurrentTask() {
			var result = RmCancelCurrentTask(handle);
			CheckResult(result);
		}

		public void RegisterResources(String[]? fileNames, String[]? serviceNames) {
			var result = RmRegisterResources(
				handle,
				(UInt32)(fileNames is null?0:fileNames.Length), fileNames,
				0,null,
				(UInt32)(serviceNames is null ? 0 : serviceNames.Length), serviceNames
			);
			CheckResult(result);
		}

		public ProcessInfo[] GetList(out RebootReason reason) {
			UInt32 needed=10;
			UInt32 size = needed;
			ProcessInfo.Native[] narr;
			for(; ;) {
				size = needed;
				narr = new ProcessInfo.Native[size];
				var result = RmGetList(handle, ref needed, ref size, narr, out reason);
				if(result == Result.MoreData) continue;
				CheckResult(result);
				break;
			}

			ProcessInfo[] marr = new ProcessInfo[needed];
			for(var i=0;i<needed;++i) {
				marr[i] = narr[i].AsNative();
			}
			return marr;
		}

		public static RestartManager JoinSession(string sessionKey) {
			var result = RmJoinSession(out var handle, sessionKey);

			CheckResult(result);
			return new RestartManager(handle);
		}

		private static void CheckResult(Result result) {
			if(result != Result.Success) throw new Exception();
		}

		[DllImport("Rstrtmgr.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern Result RmStartSession(out UIntPtr handle, UInt32 flags, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder sessionKey);

		[DllImport("Rstrtmgr.dll", ExactSpelling = true, SetLastError = true)]
		private static extern Result RmJoinSession(out UIntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string sessionKey);

		[DllImport("Rstrtmgr.dll", ExactSpelling = true, SetLastError = true)]
		private static extern Result RmEndSession(UIntPtr handle);

		[DllImport("Rstrtmgr.dll", ExactSpelling = true, SetLastError = true)]
		private static extern Result RmCancelCurrentTask(UIntPtr handle);

		[DllImport("Rstrtmgr.dll", ExactSpelling = true, SetLastError = true)]
		private static extern Result RmRegisterResources(UIntPtr handle,
			UInt32 nFiles, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] String []?fileNames,
			UInt32 nApplications, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStruct)] UniqueProcess.Native []?applications,
			UInt32 nServices, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] String []?serviceNames
			);

		[DllImport("Rstrtmgr.dll", ExactSpelling = true, SetLastError = true)]
		private static extern Result RmGetList(UIntPtr handle,
			ref UInt32 nProcInfoNeeded, ref UInt32 nProcInfo,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStruct)] ProcessInfo.Native[] affectedApps,
			out RebootReason reason
			);

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
