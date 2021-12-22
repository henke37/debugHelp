using System;

namespace Henke37.Win32.Debug {
	internal class LoadDllDebugInfo {
		internal IntPtr fileHandle;
		internal IntPtr imageBase;
		internal UInt32 debugInfoOffset;
		internal UInt32 debugInfoSize;
		internal IntPtr imageName;
		internal UInt16 imageNameIsUnicode;
	}
}