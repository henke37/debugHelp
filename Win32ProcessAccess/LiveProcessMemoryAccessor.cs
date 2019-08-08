using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Henke37.DebugHelp.Win32 {
	public sealed class LiveProcessMemoryAccessor : ProcessMemoryAccessor {
		private Process process;

		public LiveProcessMemoryAccessor(Process process) {
			this.process = process;
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[SuppressUnmanagedCodeSecurity]
		public unsafe override void ReadBytes(IntPtr addr, uint size, byte[] buff) {
			int readC;
			try { 
				fixed (Byte* buffP = buff) {
					bool success=ReadProcessMemory(process.Handle, addr, buffP, size, out readC);
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
				bool success = ReadProcessMemory(process.Handle, addr, buff, size, out int readC);
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
					bool success = WriteProcessMemory(process.Handle, (IntPtr)dstAddr, buffP, size, out var written);
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
				bool success = WriteProcessMemory(process.Handle, (IntPtr)dstAddr, (byte*)srcBuff, size, out var written);
				if(!success) throw new Win32Exception();
			} catch(Win32Exception err) when(err.NativeErrorCode == IncompleteWriteException.ErrorNumber) {
				throw new IncompleteWriteException(err);
			}
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static unsafe extern bool WriteProcessMemory(
		  IntPtr hProcess,
		  IntPtr lpBaseAddress,
		  byte* lpBuffer,
		  UInt32 nSize,
		  out IntPtr lpNumberOfBytesWritten
		);

		[DllImport("kernel32.dll", SetLastError = true)]
		unsafe static extern private bool ReadProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			[Out] void* lpBuffer,
			uint dwSize,
			out int lpNumberOfBytesRead
		);
	}
}
