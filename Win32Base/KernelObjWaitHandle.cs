using Henke37.Win32.SafeHandles;
using System;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace Henke37.Win32.Base {
	internal class KernelObjWaitHandle : WaitHandle {
		public KernelObjWaitHandle(SafeKernelObjHandle safeKernel) {
			IntPtr handle=SafeKernelObjHandle.DuplicateHandleLocal(safeKernel.DangerousGetHandle(), 0x00100000, false, SafeKernelObjHandle.DuplicateOptions.None);
			SafeWaitHandle = new SafeWaitHandle(handle, true);
		}
	}
}
