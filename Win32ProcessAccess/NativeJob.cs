using Henke37.DebugHelp.Win32.AccessRights;
using Henke37.DebugHelp.Win32.SafeHandles;
using Henke37.Win32.Base;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Henke37.DebugHelp.Win32 {
	public class NativeJob {

		private SafeJobHandle handle;

		private NativeJob(SafeJobHandle handle) {
			this.handle = handle;
		}

		[SecuritySafeCritical]
		public static unsafe NativeJob Create() {
			SafeJobHandle handle = CreateJobObjectA(null, null);
			return new NativeJob(handle);
		}

		[SecuritySafeCritical]
		public static unsafe NativeJob Create(string jobName) {
			SafeJobHandle handle = CreateJobObjectA(null, jobName);
			return new NativeJob(handle);
		}

		[SecuritySafeCritical]
		public static unsafe NativeJob Open(string jobName, JobAccessRights accessRights, bool inheritHandle=false) {
			SafeJobHandle handle = OpenJobObjectA((uint)accessRights, inheritHandle, jobName);
			return new NativeJob(handle);
		}



		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static unsafe extern SafeJobHandle CreateJobObjectA(SecurityAttributes* securityAttributes, [MarshalAs(UnmanagedType.LPStr)] string? jobName);


		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static unsafe extern SafeJobHandle OpenJobObjectA(UInt32 access, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle,[MarshalAs(UnmanagedType.LPStr)] string jobName);
	}
}
