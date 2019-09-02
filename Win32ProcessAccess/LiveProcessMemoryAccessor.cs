using Henke37.Win32.Base.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Henke37.DebugHelp.Win32 {
#if NETFRAMEWORK
	[HostProtection(Unrestricted = true, ExternalProcessMgmt = true)]
#endif
	public sealed class LiveProcessMemoryAccessor : ProcessMemoryAccessor {
		private NativeProcess process;

		public LiveProcessMemoryAccessor(NativeProcess process) {
			this.process = process;
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[SuppressUnmanagedCodeSecurity]
		public unsafe override void ReadBytes(IntPtr addr, uint size, byte[] buff) {
			int readC;
			try {
				fixed (Byte* buffP = buff) {
					bool success = ReadProcessMemory(process.handle, addr, buffP, size, out readC);
					if(!success) throw new Win32Exception();
				}
			} catch(Win32Exception err) when(err.NativeErrorCode == IncompleteReadException.ErrorNumber) {
				throw new IncompleteReadException(err);
			}

			if(readC != size) throw new IncompleteReadException();
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[SuppressUnmanagedCodeSecurity]
		public unsafe override void ReadBytes(IntPtr addr, uint size, void* buff) {
			try {
				bool success = ReadProcessMemory(process.handle, addr, buff, size, out int readC);
				if(!success) throw new Win32Exception();
				if(readC != size) throw new IncompleteReadException();
			} catch(Win32Exception err) when(err.NativeErrorCode == IncompleteReadException.ErrorNumber) {
				throw new IncompleteReadException(err);
			}
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[SuppressUnmanagedCodeSecurity]
		public override unsafe void WriteBytes(byte[] srcBuff, IntPtr dstAddr, uint size) {
			try {
				fixed (byte* buffP = srcBuff) {
					bool success = WriteProcessMemory(process.handle, (IntPtr)dstAddr, buffP, size, out var written);
					if(!success) throw new Win32Exception();
				}
			} catch(Win32Exception err) when(err.NativeErrorCode == IncompleteWriteException.ErrorNumber) {
				throw new IncompleteWriteException(err);
			}
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[SuppressUnmanagedCodeSecurity]
		public override unsafe void WriteBytes(void* srcBuff, IntPtr dstAddr, uint size) {
			try {
				bool success = WriteProcessMemory(process.handle, (IntPtr)dstAddr, (byte*)srcBuff, size, out var written);
				if(!success) throw new Win32Exception();
			} catch(Win32Exception err) when(err.NativeErrorCode == IncompleteWriteException.ErrorNumber) {
				throw new IncompleteWriteException(err);
			}
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static unsafe extern bool WriteProcessMemory(
		  SafeProcessHandle hProcess,
		  IntPtr lpBaseAddress,
		  byte* lpBuffer,
		  UInt32 nSize,
		  out IntPtr lpNumberOfBytesWritten
		);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal unsafe static extern bool ReadProcessMemory(
			SafeProcessHandle hProcess,
			IntPtr lpBaseAddress,
			[Out] void* lpBuffer,
			uint dwSize,
			out int lpNumberOfBytesRead
		);
	}
}
