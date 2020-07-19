using Henke37.DebugHelp.Win32.AccessRights;
using Henke37.DebugHelp.Win32.SafeHandles;
using Henke37.Win32.Base;
using Henke37.Win32.Base.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Henke37.DebugHelp.Win32 {

#if NETFRAMEWORK
	[HostProtection(ExternalProcessMgmt=true)]
#endif
	public class NativeJob {

		private SafeJobHandle handle;

		private NativeJob(SafeJobHandle handle) {
			this.handle = handle;
		}

		[SecuritySafeCritical]
		[SuppressUnmanagedCodeSecurity]
		public static unsafe NativeJob Create() {
			SafeJobHandle handle = CreateJobObjectA(null, null);
			return new NativeJob(handle);
		}

		[SecuritySafeCritical]
		[SuppressUnmanagedCodeSecurity]
		public static unsafe NativeJob Create(string jobName) {
			SafeJobHandle handle = CreateJobObjectA(null, jobName);
			return new NativeJob(handle);
		}

		[SecuritySafeCritical]
		[SuppressUnmanagedCodeSecurity]
		public static unsafe NativeJob Open(string jobName, JobAccessRights accessRights, bool inheritHandle=false) {
			SafeJobHandle handle = OpenJobObjectA((uint)accessRights, inheritHandle, jobName);
			return new NativeJob(handle);
		}

		[SecuritySafeCritical]
		public void AttachProcess(NativeProcess process) {
			AttachProcess(process.handle);
		}

		[SecuritySafeCritical]
		[SuppressUnmanagedCodeSecurity]
		internal void AttachProcess(SafeProcessHandle processHandle) {
			bool success = AssignProcessToJobObject(handle, processHandle);
			if(!success) throw new Win32Exception();
		}

		internal unsafe T QueryInformationJob<T>(JobInformationClass infoClass, ref T buff) where T : unmanaged {
			fixed(void* buffP = &buff) {
				bool success = QueryInformationJobObject(handle, infoClass, buffP, (uint)sizeof(T), out _);
				if(!success) throw new Win32Exception();
				return buff;
			}
		}

		[SecuritySafeCritical]
		void Terminate(UInt32 exitCode) {
			TerminateJobObject(handle, exitCode);
		}


		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static unsafe extern SafeJobHandle CreateJobObjectA(SecurityAttributes* securityAttributes, [MarshalAs(UnmanagedType.LPStr)] string? jobName);


		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static unsafe extern SafeJobHandle OpenJobObjectA(UInt32 access, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle,[MarshalAs(UnmanagedType.LPStr)] string jobName);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static unsafe extern bool AssignProcessToJobObject(SafeJobHandle jobHandle, SafeProcessHandle procHandle);

		[DllImport("Ntdll.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool QueryInformationJobObject(SafeJobHandle handle, JobInformationClass informationClass, void* buffer, uint bufferLength, out uint returnLength);

		[DllImport("Ntdll.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool TerminateJobObject(SafeJobHandle handle, UInt32 exitCode);
	}
}
