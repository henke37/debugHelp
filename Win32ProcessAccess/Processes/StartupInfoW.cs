using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Processes {
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct StartupInfoW {
		private UInt32 cb;
		internal Char *lpReserved;
		public Char *lpDesktop;
		public Char *lpTitle;
		public Int32 dwX;
		public Int32 dwY;
		public UInt32 dwXSize;
		public UInt32 dwYSize;
		public UInt32 dwXCountChars;
		public UInt32 dwYCountChars;
		public UInt32 dwFillAttribute;
		public StartupInfoFlags dwFlags;
		public UInt16 wShowWindow;
		private UInt16 cbReserved2;
		private void *lpReserved2;
		public IntPtr hStdInput;
		public IntPtr hStdOutput;
		public IntPtr hStdError;

		internal StartupInfoW(StartupInfoFlags flags) {
			lpReserved = null;
			lpDesktop = null;
			lpTitle = null;
			dwX = 0;
			dwY = 0;
			dwXSize = 0;
			dwYSize = 0;
			dwXCountChars = 0;
			dwYCountChars = 0;
			dwFillAttribute = 0;
			dwFlags = flags;
			wShowWindow = 0;
			cbReserved2 = 0;
			lpReserved2 = null;
			hStdInput = default;
			hStdOutput = default;
			hStdError = default;

			ValidateFlags(flags);

			cb = (uint)sizeof(StartupInfoW);
		}

		private static void ValidateFlags(StartupInfoFlags flags) {
			if((flags & StartupInfoFlags.UseHotKey)!=0 && (flags & StartupInfoFlags.UseSTDHandles)!=0) {
				throw new ArgumentException("Invalid flags combination");
			}
		}
	}
}