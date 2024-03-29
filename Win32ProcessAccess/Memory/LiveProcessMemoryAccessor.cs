﻿using Henke37.Win32.Processes;
using Henke37.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Henke37.DebugHelp;

namespace Henke37.Win32.Memory {
#if NETFRAMEWORK
	[HostProtection(Unrestricted = true, ExternalProcessMgmt = true)]
#endif
	public sealed class LiveProcessMemoryAccessor : ProcessMemoryAccessor {
		private SafeProcessHandle handle;

		public LiveProcessMemoryAccessor(NativeProcess process) {
			this.handle = process.handle;
		}

		public LiveProcessMemoryAccessor(SafeProcessHandle handle) {
			if(handle == null) throw new ArgumentNullException(nameof(handle));
			this.handle = handle;
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public unsafe override void ReadBytes(IntPtr addr, uint size, byte[] buff) {
			int readC;
			try {
				fixed (Byte* buffP = buff) {
					bool success = ReadProcessMemory(handle, addr, buffP, size, out readC);
					if(!success) throw new Win32Exception();
				}
			} catch(Win32Exception err) when(err.NativeErrorCode == IncompleteReadException.ErrorNumber) {
				throw new IncompleteReadException(err);
			}

			if(readC != size) throw new IncompleteReadException();
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public unsafe override void ReadBytes(IntPtr addr, uint size, void* buff) {
			try {
				bool success = ReadProcessMemory(handle, addr, buff, size, out int readC);
				if(!success) throw new Win32Exception();
				if(readC != size) throw new IncompleteReadException();
			} catch(Win32Exception err) when(err.NativeErrorCode == IncompleteReadException.ErrorNumber) {
				throw new IncompleteReadException(err);
			}
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public override unsafe void WriteBytes(byte[] srcBuff, IntPtr dstAddr, uint size) {
			try {
				fixed (byte* buffP = srcBuff) {
					bool success = WriteProcessMemory(handle, (IntPtr)dstAddr, buffP, size, out var written);
					if(!success) throw new Win32Exception();
				}
			} catch(Win32Exception err) when(err.NativeErrorCode == IncompleteWriteException.ErrorNumber) {
				throw new IncompleteWriteException(err);
			}
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public override unsafe void WriteBytes(void* srcBuff, IntPtr dstAddr, uint size) {
			try {
				bool success = WriteProcessMemory(handle, (IntPtr)dstAddr, (byte*)srcBuff, size, out var written);
				if(!success) throw new Win32Exception();
			} catch(Win32Exception err) when(err.NativeErrorCode == IncompleteWriteException.ErrorNumber) {
				throw new IncompleteWriteException(err);
			}
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		internal static unsafe extern bool WriteProcessMemory(
		  SafeProcessHandle hProcess,
		  IntPtr lpBaseAddress,
		  byte* lpBuffer,
		  UInt32 nSize,
		  out IntPtr lpNumberOfBytesWritten
		);

		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		internal unsafe static extern bool ReadProcessMemory(
			SafeProcessHandle hProcess,
			IntPtr lpBaseAddress,
			[Out] void* lpBuffer,
			uint dwSize,
			out int lpNumberOfBytesRead
		);
	}
}
