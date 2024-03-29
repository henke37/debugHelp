﻿using Henke37.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace Henke37.Win32.Memory {
	public class MemoryResourceNotification : IDisposable {
		SafeMemoryResourceNotificationHandle handle;

		public MemoryResourceNotification(MemoryNotificationType type) {
			handle= CreateMemoryResourceNotification((uint)type);
			if(handle.IsInvalid) throw new Win32Exception();
		}

		public bool Status {
			get {
				var success = QueryMemoryResourceNotification(handle, out bool status);
				if(!success) throw new Win32Exception();
				return status;
			}
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		internal static unsafe extern SafeMemoryResourceNotificationHandle CreateMemoryResourceNotification(uint type);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static unsafe extern bool QueryMemoryResourceNotification(SafeMemoryResourceNotificationHandle handle, [MarshalAs(UnmanagedType.Bool)] out bool status);

		public void Dispose() {
			((IDisposable)handle).Dispose();
		}
	}

	public enum MemoryNotificationType : uint {
		LowMemory=0,
		HighMemory=1
	}
}
