using Henke37.Win32.AccessRights;
using Henke37.Win32.Processes;
using Henke37.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Henke37.Win32.Jobs {

#if NETFRAMEWORK
	[HostProtection(ExternalProcessMgmt=true)]
#endif
	public class NativeJob : IDisposable {

		private SafeJobHandle handle;

		private NativeJob(SafeJobHandle handle) {
			this.handle = handle;
		}

		[SecuritySafeCritical]
		[SuppressUnmanagedCodeSecurity]
		public static unsafe NativeJob Create() {
			SafeJobHandle handle = CreateJobObjectW(null, null);
			if(handle.IsInvalid) throw new Win32Exception();
			return new NativeJob(handle);
		}

		[SecuritySafeCritical]
		[SuppressUnmanagedCodeSecurity]
		public static unsafe NativeJob Create(string jobName) {
			SafeJobHandle handle = CreateJobObjectW(null, jobName);
			if(handle.IsInvalid) throw new Win32Exception();
			return new NativeJob(handle);
		}

		[SecuritySafeCritical]
		[SuppressUnmanagedCodeSecurity]
		public static unsafe NativeJob Open(string jobName, JobAccessRights accessRights = JobAccessRights.All, bool inheritHandle=false) {
			SafeJobHandle handle = OpenJobObjectW((uint)accessRights, inheritHandle, jobName);
			if(handle.IsInvalid) throw new Win32Exception();
			return new NativeJob(handle);
		}

		[SecuritySafeCritical]
		[SuppressUnmanagedCodeSecurity]
		public void AttachProcess(NativeProcess process) {
			AttachProcess(process.handle);
		}

		[SecuritySafeCritical]
		[SuppressUnmanagedCodeSecurity]
		internal void AttachProcess(SafeProcessHandle processHandle) {
			bool success = AssignProcessToJobObject(handle, processHandle);
			if(!success) throw new Win32Exception();
		}

		[SuppressUnmanagedCodeSecurity]
		internal unsafe T QueryInformationJob<T>(JobInformationClass infoClass, out T buff) where T : unmanaged {
			fixed(void* buffP = &buff) {
				bool success = QueryInformationJobObject(handle, infoClass, buffP, (uint)sizeof(T), out _);
				if(!success) throw new Win32Exception();
				return buff;
			}
		}

		[SuppressUnmanagedCodeSecurity]
		internal unsafe void SetInformationJob<T>(JobInformationClass infoClass, ref T buff) where T : unmanaged {
			fixed(void* buffP = &buff) {
				bool success = SetInformationJobObject(handle, infoClass, buffP, (uint)sizeof(T));
				if(!success) throw new Win32Exception();
			}
		}

		[SecuritySafeCritical]
		[SuppressUnmanagedCodeSecurity]
		void Terminate(UInt32 exitCode) {
			TerminateJobObject(handle, exitCode);
		}

		public BasicLimitInformation BasicLimitInformation {
			get {
				BasicLimitInformation.Native native;
				QueryInformationJob(JobInformationClass.BasicLimitInformation, out native);
				return native.AsManaged();
			}
			set {
				BasicLimitInformation.Native native = new BasicLimitInformation.Native(value);
				SetInformationJob(JobInformationClass.BasicLimitInformation, ref native);
			}
		}

		public ExtendedLimitInformation ExtendedLimitInformation {
			get {
				ExtendedLimitInformation.Native native;
				QueryInformationJob(JobInformationClass.ExtendedLimitInformation, out native);
				return native.AsManaged();
			}
			set {
				ExtendedLimitInformation.Native native = new ExtendedLimitInformation.Native(value);
				SetInformationJob(JobInformationClass.ExtendedLimitInformation, ref native);
			}
		}

		public BasicUIRestrictions BasicUIRestrictions {
			get {
				BasicUIRestrictionsStruct native;
				QueryInformationJob(JobInformationClass.BasicUIRestrictions, out native);
				return native.restrictions;
			}
			set {
				BasicUIRestrictionsStruct native=new BasicUIRestrictionsStruct(value);
				SetInformationJob(JobInformationClass.BasicUIRestrictions, ref native);
			}
		}

		public EndOfJobTimeAction EndOfJobTimeAction {
			get {
				EndOfJobTimeActionStruct native;
				QueryInformationJob(JobInformationClass.EndOfJobTimeInformation, out native);
				return native.endOfJobTimeAction;
			}
			set {
				EndOfJobTimeActionStruct native=new EndOfJobTimeActionStruct(value);
				SetInformationJob(JobInformationClass.EndOfJobTimeInformation, ref native);
			}
		}

		public NetRateControlInformation NetRateControlInformation {
			get {
				NetRateControlInformation native;
				QueryInformationJob(JobInformationClass.NetRateControlInformation, out native);
				return native;
			}
			set {
				SetInformationJob(JobInformationClass.NetRateControlInformation, ref value);
			}
		}

		public CpuRateControlInformation CpuRateControlInformation {
			get {
				CpuRateControlInformation.Native native;
				QueryInformationJob(JobInformationClass.CpuRateControlInformation, out native);
				return new CpuRateControlInformation(native);
			}
			set {
				CpuRateControlInformation.Native native = new CpuRateControlInformation.Native(value);
				SetInformationJob(JobInformationClass.CpuRateControlInformation, ref native);
			}
		}

		[SuppressUnmanagedCodeSecurity]
		public bool IsProcessInJob(NativeProcess process) {
			bool success = IsProcessInJobNative(process.handle, handle, out bool result);
			if(!success) throw new Win32Exception();
			return result;
		}

		internal void GrantAccessToUSERHandle(IntPtr userHandle, bool grant) {
			bool success = UserHandleGrantAccess(userHandle, handle, grant);
			if(!success) throw new Win32Exception();
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
		internal static unsafe extern SafeJobHandle CreateJobObjectW(SecurityAttributes* securityAttributes, [MarshalAs(UnmanagedType.LPWStr)] string? jobName);


		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
		internal static unsafe extern SafeJobHandle OpenJobObjectW(UInt32 access, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle,[MarshalAs(UnmanagedType.LPWStr)] string jobName);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static unsafe extern bool AssignProcessToJobObject(SafeJobHandle jobHandle, SafeProcessHandle procHandle);

		[DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool QueryInformationJobObject(SafeJobHandle handle, JobInformationClass informationClass, void* buffer, uint bufferLength, out uint returnLength);


		[DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool SetInformationJobObject(SafeJobHandle handle, JobInformationClass informationClass, void* buffer, uint bufferLength);

		[DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool TerminateJobObject(SafeJobHandle handle, UInt32 exitCode);

		[DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "IsProcessInJob")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool IsProcessInJobNative(SafeProcessHandle processHandle, SafeJobHandle jobHandle, [MarshalAs(UnmanagedType.Bool)] out bool result);


		[DllImport("User32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool UserHandleGrantAccess(IntPtr userHandle, SafeJobHandle jobHandle, [MarshalAs(UnmanagedType.Bool)] bool grant);

		public void Dispose() => handle.Dispose();
	}
}
