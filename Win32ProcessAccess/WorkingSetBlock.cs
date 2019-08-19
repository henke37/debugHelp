using System;

namespace Henke37.DebugHelp.Win32 {
	public class WorkingSetBlock {
		public WorkingSetBlockPageProtectionFlags Protection;
		public int ShareCount;
		public IntPtr VirtualPage;

#if x86
		public WorkingSetBlock(int v) {
			Protection = (WorkingSetBlockPageProtectionFlags)(v & 31);
			ShareCount = (v >> 5) & 7;
			VirtualPage = (IntPtr)(v >> 12);
		}
#endif

		[Flags]
		public enum WorkingSetBlockPageProtectionFlags {
			None = 0,
			ReadOnly = 1,
			Executable = 2,
			ReadWrite = 4,
			CopyOnWrite = 5,
			NonCacheable = 8,
			GuardPage = 16
		}
	}
}