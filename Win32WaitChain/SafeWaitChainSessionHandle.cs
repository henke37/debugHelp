using System;
using System.Runtime.InteropServices;

namespace Win32WaitChain {
	internal class SafeWaitChainSessionHandle : SafeHandle {
		private static readonly IntPtr InvalidHandleValue=new IntPtr(0);

		private SafeWaitChainSessionHandle() : base(InvalidHandleValue, true) {
		}

		public override bool IsInvalid => handle == InvalidHandleValue;

		protected override bool ReleaseHandle() {
			CloseThreadWaitChainSession(handle);
			return true;
		}

		[DllImport("Advapi32.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern void CloseThreadWaitChainSession(IntPtr handle);
	}
}
