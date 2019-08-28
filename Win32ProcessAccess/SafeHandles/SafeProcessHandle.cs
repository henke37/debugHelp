using System;
using System.Security.Permissions;

namespace Henke37.DebugHelp.Win32.SafeHandles {
	internal sealed class SafeProcessHandle : SafeKernelObjHandle, IEquatable<SafeProcessHandle> {

		public static SafeProcessHandle CurrentProcess {
#if NETFRAMEWORK
			[HostProtection(SelfAffectingProcessMgmt = true)]
#endif
			get {
				return new SafeProcessHandle((IntPtr)(-1), false);
			}
		}

		internal SafeProcessHandle() : base(true) {
		}

		public SafeProcessHandle(IntPtr handle, bool ownsHandle = true) : base(ownsHandle) {
			this.handle = handle;
		}

		public bool Equals(SafeProcessHandle other) {
			if(other.handle == handle) return true;
			return CompareObjectHandles(other.handle, handle);
		}

	}
}
