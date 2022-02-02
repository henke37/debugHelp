using Henke37.Win32.Clone.QueryStructs;
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

		internal SafeProcessHandle DuplicatedProcessHandle {
			get {
				QueryInformation(QueryInformationClass.VA_CLONE_INFORMATION, out VA_CLONE_INFORMATION ci);
				return new SafeProcessHandle(ci.Handle);
			}
		}

		internal Int32 AuxPageCount {
			get {
				QueryInformation(QueryInformationClass.AUXILIARY_PAGES_INFORMATION, out int Count);
				return Count;
			}
		}

		internal Int32 RegionCount {
			get {
				QueryInformation(QueryInformationClass.VA_SPACE_INFORMATION, out int Count);
				return Count;
			}
		}

		internal Int32 HandleCount {
			get {
				QueryInformation(QueryInformationClass.HANDLE_INFORMATION, out int Count);
				return Count;
			}
		}

		internal VA_CLONE_INFORMATION ThreadData {
			get {
				QueryInformation(QueryInformationClass.THREAD_INFORMATION, out VA_CLONE_INFORMATION td);
				return td;
			}
		}

		internal PERFORMANCE_COUNTERS PerformanceCounters {
			get {
				QueryInformation(QueryInformationClass.PERFORMANCE_COUNTERS, out PERFORMANCE_COUNTERS.Native native);
				return native.AsManaged();
			}
		}

		[SuppressUnmanagedCodeSecurity]
		[SecuritySafeCritical]
		internal unsafe T QueryInformation<T>(QueryInformationClass infoClass, out T buff) where T : unmanaged {
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
