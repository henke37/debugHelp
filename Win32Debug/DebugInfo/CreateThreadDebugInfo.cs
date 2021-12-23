using System;

namespace Henke37.Win32.Debug.Info {
	internal struct CreateThreadDebugInfo {
		internal IntPtr threadHandle;
		internal IntPtr localBase;
		internal IntPtr startAddress;
	}
}