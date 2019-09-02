using Henke37.DebugHelp.Win32.AccessRights;
using Henke37.DebugHelp.Win32.SafeHandles;
using Henke37.Win32.Base;
using Henke37.Win32.Base.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
	public class FileMapping : IDisposable {
		internal SafeFileMappingHandle handle;

		internal FileMapping(SafeFileMappingHandle handle) {
			this.handle = handle;
		}

		public static unsafe FileMapping CreateVirtualMapping(MemoryProtection memProtection, FileMappingFlags flags, UInt64 size, string name) {
			SafeFileMappingHandle handle = CreateFileMappingA(
				SafeFileObjectHandle.InvalidHandle,
				null,
				(uint)memProtection | (uint)flags,
				(uint)(size >> 32), (uint)(size & 0xFFFFFFFF),
				name
			);
			if(handle.IsInvalid) throw new Win32Exception();
			return new FileMapping(handle);
		}

		public static unsafe FileMapping OpenMapping(string name,bool inherit=false, FileMappingAccessRights accessRights=FileMappingAccessRights.All) {
			SafeFileMappingHandle handle = OpenFileMappingA((uint)accessRights,inherit,name);
			if(handle.IsInvalid) throw new Win32Exception();
			return new FileMapping(handle);
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Ansi)]
		internal static unsafe extern SafeFileMappingHandle CreateFileMappingA(SafeFileObjectHandle fileHandle, [Optional] SecurityAttributes* securityAttributes, UInt32 flProtect, UInt32 dwMaximumSizeHigh, UInt32 dwMaximumSizeLow, [MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Ansi)]
		internal static unsafe extern SafeFileMappingHandle OpenFileMappingA(UInt32 access, [MarshalAs(UnmanagedType.Bool)] bool inherit, [MarshalAs(UnmanagedType.LPStr)] string name);

		public void Dispose() {
			((IDisposable)handle).Dispose();
		}
	}
}
