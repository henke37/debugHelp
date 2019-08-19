using System;

namespace Henke37.DebugHelp.Win32 {
	public class WorkingSetBlock {
		public WorkingSetBlockFlags Flags;
		public WorkingSetBlockPageProtectionFlags Protection;
		public int ShareCount;
		public IntPtr VirtualPage;

		[Flags]
		public enum WorkingSetBlockPageProtectionFlags {
			None = 0
		}

		[Flags]
		public enum WorkingSetBlockFlags {
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