﻿using System;
using System.Security;
using System.Security.Permissions;

namespace Henke37.Win32.SafeHandles {
	class SafeJobHandle : SafeKernelObjHandle, IEquatable<SafeJobHandle> {
		private SafeJobHandle() : base(true) {
		}

		internal SafeJobHandle(IntPtr handle, bool ownsHandle = true) : base(handle, ownsHandle) {
		}

#if NETFRAMEWORK
		[HostProtection(MayLeakOnAbort = true)]
#endif
		internal static SafeJobHandle DuplicateFrom(IntPtr handle) {
			return new SafeJobHandle(DuplicateHandleLocal(handle, 0, false, DuplicateOptions.SameAccess), true);
		}
#if NETFRAMEWORK
		[HostProtection(MayLeakOnAbort = true)]
#endif
		internal static SafeJobHandle DuplicateFrom(IntPtr handle, uint accessRights) {
			return new SafeJobHandle(DuplicateHandleLocal(handle, accessRights, false, DuplicateOptions.None), true);
		}

		[SecuritySafeCritical]
		public bool Equals(SafeJobHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}
	}
}
