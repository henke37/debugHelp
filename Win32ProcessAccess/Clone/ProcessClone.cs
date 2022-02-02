using Henke37.Win32.Processes;
using Henke37.Win32.SafeHandles;
using Henke37.Win32.Threads;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace Henke37.Win32.Clone {
	public class ProcessClone : IDisposable {
		internal SafeProcessCloneHandle Handle;

		internal ProcessClone(SafeProcessCloneHandle Handle) {
			this.Handle = Handle;
		}

		[SuppressUnmanagedCodeSecurity]
		public static ProcessClone CloneProcess(NativeProcess proc, CloneFlags flags, ContextFlags contextFlags = 0) {
			var ret = PssCaptureSnapshot(proc.handle, flags, contextFlags, out SafeProcessCloneHandle clonedProc);
			if(ret != 0) throw new Win32Exception(ret);
			return new ProcessClone(clonedProc);
		}

		[SuppressUnmanagedCodeSecurity]
		[SecuritySafeCritical]
		internal unsafe T QueryInformation<T>(QueryInformationClass infoClass, ref T buff) where T : unmanaged {
			fixed(void* buffP = &buff) {
				var ret = PssQuerySnapshot(Handle, infoClass, buffP, (uint)sizeof(T));
				if(ret != 0) throw new Win32Exception(ret);
				return buff;
			}
		}

		public void Dispose() {
			Handle.Dispose();
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern Int32 PssCaptureSnapshot(SafeProcessHandle originalProcess, CloneFlags flags, ContextFlags contextFlags, out SafeProcessCloneHandle clonedProc);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		internal static unsafe extern Int32 PssQuerySnapshot(SafeProcessCloneHandle clonedProc, QueryInformationClass informationClass, void * buffer, UInt32 buffSize);

	}
}
