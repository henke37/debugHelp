using System;
using System.Text;

namespace Henke37.Win32.Memory {
	public class WorkingSetBlock {
		public WorkingSetBlockPageProtectionFlags Protection;
		public bool Shared;
		public int ShareCount;
		public IntPtr VirtualPage;

#if x86
		public WorkingSetBlock(int v) {
			Protection = (WorkingSetBlockPageProtectionFlags)(v & 31);
			ShareCount = (v >> 5) & 7;
			Shared = ((v >> 8) & 1) == 1;
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

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			if((Protection & WorkingSetBlockPageProtectionFlags.CopyOnWrite) == WorkingSetBlockPageProtectionFlags.CopyOnWrite) {
				sb.Append("CW");
			} else if((Protection & WorkingSetBlockPageProtectionFlags.ReadOnly) != 0) {
				sb.Append("RO");
			} else if((Protection & WorkingSetBlockPageProtectionFlags.ReadWrite) != 0) {
				sb.Append("RW");
			}
			if((Protection & WorkingSetBlockPageProtectionFlags.NonCacheable) != 0) {
				sb.Append("NC");
			}
			if((Protection & WorkingSetBlockPageProtectionFlags.GuardPage) != 0) {
				sb.Append("GP");
			}
			if(Shared) {
				sb.Append(" SH");
			}
			sb.AppendFormat(" 0x{0:x}", VirtualPage);
			return sb.ToString();
		}
	}
}