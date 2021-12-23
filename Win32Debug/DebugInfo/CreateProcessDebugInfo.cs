using System;

namespace Henke37.Win32.Debug.Info {
	internal struct CreateProcessDebugInfo {
		internal IntPtr fileHandle;
		internal IntPtr processHandle;
		internal IntPtr threadHandle;
		internal IntPtr imageBase;
		internal UInt32 debugInfoOffset;
		internal UInt32 debugInfoSize;
		internal IntPtr localBase;
		internal IntPtr startAddress;
		internal IntPtr imageName;
		internal UInt16 imageNameIsUnicode;
	}
}