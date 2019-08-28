using Henke37.DebugHelp.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
	public class FileMapping : IDisposable {
		private SafeFileMappingHandle handle;

		internal FileMapping(SafeFileMappingHandle handle) {
			this.handle = handle;
		}

		public static unsafe FileMapping CreateVirtualMapping(MemoryProtection memProtection, FileMappingFlags flags, uint high, uint low, string name) {
			SafeFileMappingHandle handle = CreateFileMappingA(SafeFileObjectHandle.InvalidHandle, null, (uint)memProtection | (uint)flags, high, low, name);
			if(handle.IsInvalid) throw new Win32Exception();
			return new FileMapping(handle);
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Ansi)]
		internal static unsafe extern SafeFileMappingHandle CreateFileMappingA(SafeFileObjectHandle fileHandle, [Optional] SecurityAttributes* securityAttributes, UInt32 flProtect, UInt32 dwMaximumSizeHigh, UInt32 dwMaximumSizeLow, [MarshalAs(UnmanagedType.LPStr)] string name);

		public void Dispose() {
			((IDisposable)handle).Dispose();
		}
	}
}
