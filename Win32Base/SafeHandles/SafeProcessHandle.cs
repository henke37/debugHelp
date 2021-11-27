using System;
using System.Security.Permissions;

namespace Henke37.Win32.SafeHandles {
	public sealed class SafeProcessHandle : SafeKernelObjHandle, IEquatable<SafeProcessHandle>, IEquatable<IntPtr>, IEquatable<Microsoft.Win32.SafeHandles.SafeProcessHandle> {

		public static SafeProcessHandle CurrentProcess {
#if NETFRAMEWORK
			[HostProtection(SelfAffectingProcessMgmt = true)]
#endif
			get {
				return new SafeProcessHandle((IntPtr)(-1), false);
			}
		}
		private SafeProcessHandle() : base(true) {
		}

		internal SafeProcessHandle(IntPtr handle, bool ownsHandle = true) : base(handle,ownsHandle) {
		}

#if NETFRAMEWORK
		[HostProtection(MayLeakOnAbort = true)]
#endif
		internal static SafeProcessHandle DuplicateFrom(IntPtr handle) {
			return new SafeProcessHandle(DuplicateHandleLocal(handle, 0, false, DuplicateOptions.SameAccess), true);
		}
#if NETFRAMEWORK
		[HostProtection(MayLeakOnAbort = true)]
#endif
		internal static SafeProcessHandle DuplicateFrom(IntPtr handle, uint accessRights) {
			return new SafeProcessHandle(DuplicateHandleLocal(handle, accessRights, false, DuplicateOptions.None), true);
		}

#if NETFRAMEWORK
		[HostProtection(MayLeakOnAbort = true)]
#endif
		public static SafeProcessHandle DuplicateFrom(Microsoft.Win32.SafeHandles.SafeProcessHandle safeHandle) {
			return new SafeProcessHandle(DuplicateHandleLocal(safeHandle.DangerousGetHandle(), 0, false, DuplicateOptions.SameAccess), true);
		}
#if NETFRAMEWORK
		[HostProtection(MayLeakOnAbort = true)]
#endif
		public static SafeProcessHandle DuplicateFrom(Microsoft.Win32.SafeHandles.SafeProcessHandle safeHandle, uint accessRights) {
			return new SafeProcessHandle(DuplicateHandleLocal(safeHandle.DangerousGetHandle(), accessRights, false, DuplicateOptions.None), true);
		}

		public bool Equals(SafeProcessHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}

		public bool Equals(IntPtr other) {
			if(other == handle) return true;
			return CompareObjectHandles(other, handle);
		}

		public bool Equals(Microsoft.Win32.SafeHandles.SafeProcessHandle other) {
			return CompareObjectHandles(other.DangerousGetHandle(), handle);
		}

		public override bool IsInvalid {
			get {
				if(handle.ToInt32() == -1) return false;
				return base.IsInvalid;
			}
		}
	}
}
