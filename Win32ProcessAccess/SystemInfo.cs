using Henke37.Win32.SafeHandles;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Henke37.Win32 {
	public class SystemInfo {
		public static SystemInfo Data => GetSystemInfo();

		public ProcessorArchitectureType ProcessorArchitecture;
		public UInt32 PageSize;
		public IntPtr MinimumApplicationAddress;
		public IntPtr MaximumApplicationAddress;
		public UInt32 ActiveProcessorMask;
		public UInt32 NumberOfProcessors;
		public UInt32 AllocationGranularity;
		public UInt16 ProcessorLevel;
		public UInt16 ProcessorRevision;

		private unsafe static SystemInfo GetSystemInfo() {
			Native native;
			GetSystemInfo(&native);

			return native.ToManaged();
		}

		[StructLayout(LayoutKind.Sequential)]
		internal unsafe struct Native {
			internal UInt16 wProcessorArchitecture;
			internal UInt16 wReserved;
			internal UInt32 dwPageSize;
			internal IntPtr lpMinimumApplicationAddress;
			internal IntPtr lpMaximumApplicationAddress;
			internal UInt32 dwActiveProcessorMask;
			internal UInt32 dwNumberOfProcessors;
			internal UInt32 dwAllocationGranularity;
			internal UInt16 wProcessorLevel;
			internal UInt16 wProcessorRevision;

			internal SystemInfo ToManaged() {
				return new SystemInfo {
					ProcessorArchitecture = (ProcessorArchitectureType)(wProcessorArchitecture),
					PageSize = dwPageSize,
					MinimumApplicationAddress = lpMinimumApplicationAddress,
					MaximumApplicationAddress = lpMaximumApplicationAddress,
					ActiveProcessorMask = dwActiveProcessorMask,
					NumberOfProcessors = dwNumberOfProcessors,
					AllocationGranularity = dwAllocationGranularity,
					ProcessorLevel = wProcessorLevel,
					ProcessorRevision = wProcessorRevision
				};
			}
		}

		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
		internal static unsafe extern bool GetSystemInfo(Native* native);

		public enum ProcessorArchitectureType : UInt16 {
			AMD64 = 9,
			ARM = 5,
			ARM64=12,
			IA64=6,
			X86=0,
			Unknown = 0xFFFF
		}

#if x64
		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
		internal static unsafe extern bool GetNativeSystemInfo(Native* native);
#endif
	}
}
