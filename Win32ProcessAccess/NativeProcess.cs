using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.DebugHelp.Win32 {
	public class NativeProcess : IDisposable {
		internal SafeProcessHandle handle;

		internal NativeProcess(SafeProcessHandle handle) {
			if(handle.IsInvalid) throw new ArgumentException("Handle must be valid!", nameof(handle));
			this.handle = handle;
		}

		public static NativeProcess Open(int processId, ProcessAccessRights rights = ProcessAccessRights.All) {
			var proc = Open(processId, out var firstThread, rights);
			firstThread.Close();
			return proc;
		}
		public static NativeProcess Open(int processId, out NativeThread firstThread, ProcessAccessRights rights = ProcessAccessRights.All) {
			throw new NotImplementedException();
		}

		public void Dispose() => handle.Dispose();
		public void Close() => handle.Close();

		public static NativeProcess FromProcess(Process stdProcess) {
			return new NativeProcess(new SafeProcessHandle(stdProcess.Handle, false));
		}

		public static implicit operator NativeProcess(Process stdProcess) => FromProcess(stdProcess);
	}
}
