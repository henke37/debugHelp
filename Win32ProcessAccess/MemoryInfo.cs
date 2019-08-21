using System;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
	public class MemoryInfo {
		public UInt32 PageFaultCount;
		public UInt32 PeakWorkingSetSize;
		public UInt32 WorkingSetSize;
		public UInt32 QuotaPeakPagedPoolUsage;
		public UInt32 QuotaPagedPoolUsage;
		public UInt32 QuotaPeakNonPagedPoolUsage;
		public UInt32 QuotaNonPagedPoolUsage;
		public UInt32 PagefileUsage;
		public UInt32 PeakPagefileUsage;
		public UInt32 PrivateUsage;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct Native {
			internal UInt32 cb;
			UInt32 PageFaultCount;
			UInt32 PeakWorkingSetSize;
			UInt32 WorkingSetSize;
			UInt32 QuotaPeakPagedPoolUsage;
			UInt32 QuotaPagedPoolUsage;
			UInt32 QuotaPeakNonPagedPoolUsage;
			UInt32 QuotaNonPagedPoolUsage;
			UInt32 PagefileUsage;
			UInt32 PeakPagefileUsage;
			UInt32 PrivateUsage;

			internal MemoryInfo AsManaged() {
				return new MemoryInfo() {
					PageFaultCount = PageFaultCount,
					PeakWorkingSetSize = PeakWorkingSetSize,
					WorkingSetSize = WorkingSetSize,
					QuotaPeakPagedPoolUsage = QuotaPeakPagedPoolUsage,
					QuotaPagedPoolUsage = QuotaPagedPoolUsage,
					QuotaPeakNonPagedPoolUsage = QuotaPeakNonPagedPoolUsage,
					QuotaNonPagedPoolUsage = QuotaNonPagedPoolUsage,
					PagefileUsage = PagefileUsage,
					PeakPagefileUsage = PeakPagefileUsage,
					PrivateUsage = PrivateUsage
				};
			}
		}
	}
}