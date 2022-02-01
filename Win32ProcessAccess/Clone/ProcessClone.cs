using Henke37.Win32.Processes;
using Henke37.Win32.SafeHandles;
using Henke37.Win32.Threads;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Clone {
	public class ProcessClone : IDisposable {
		internal SafeProcessCloneHandle Handle;

		internal ProcessClone(SafeProcessCloneHandle Handle) {
			this.Handle = Handle;
		}

		public static ProcessClone CloneProcess(NativeProcess proc, CloneFlags flags, ContextFlags contextFlags = 0) {
			var ret = PssCaptureSnapshot(proc.handle, flags, contextFlags, out SafeProcessCloneHandle clonedProc);
			if(ret != 0) throw new Win32Exception(ret);
			return new ProcessClone(clonedProc);
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern Int32 PssCaptureSnapshot(SafeProcessHandle originalProcess, CloneFlags flags, ContextFlags contextFlags, out SafeProcessCloneHandle clonedProc);

		public void Dispose() {
			Handle.Dispose();
		}
	}
}
