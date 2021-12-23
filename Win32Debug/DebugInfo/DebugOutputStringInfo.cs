using System;

namespace Henke37.Win32.Debug.Info {
	internal struct DebugOutputStringInfo {
		internal IntPtr dataAddress;
		internal UInt16 isUnicode;
		internal UInt16 truncatedStringLength;
	}
}