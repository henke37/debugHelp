using System;

namespace Henke37.Win32.Processes {
	internal unsafe struct StartupInfoW {
		UInt32 cb;
		internal Char *lpReserved;
		internal Char *lpDesktop;
		internal Char *lpTitle;
		internal Int32 dwX;
		internal Int32 dwY;
		internal UInt32 dwXSize;
		internal UInt32 dwYSize;
		internal UInt32 dwXCountChars;
		internal UInt32 dwYCountChars;
		internal UInt32 dwFillAttribute;
		internal StartupInfoFlags dwFlags;
		internal UInt16 wShowWindow;
		internal UInt16 cbReserved2;
		internal void *lpReserved2;
		internal IntPtr hStdInput;
		internal IntPtr hStdOutput;
		internal IntPtr hStdError;

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

			cb = (uint)sizeof(StartupInfoW);
		}
	}
}