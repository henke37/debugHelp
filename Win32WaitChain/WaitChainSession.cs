using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Henke37.Win32.WaitChain {
	public class WaitChainSession {
		internal SafeWaitChainSessionHandle handle;

		public unsafe WaitChainSession() {
			handle = OpenThreadWaitChainSession(
				(uint)WCT_SESSION_OPEN_FLAGS.WCT_ASYNC_OPEN_FLAG,
				AsyncOperation.asyncCallback
			);
			if(handle.IsInvalid) {
				throw new Win32Exception();
			}
		}

		public async Task<List<WaitChanNodeInfo>> GetChainAsync(GetWaitChainFlags flags, UInt32 threadId) {
			var operation = new AsyncOperation(this, flags, threadId);
			try {
				return await operation.GetTask();
			} finally {
				operation.Dispose();
			}
		}

		internal unsafe delegate void Pwaitchaincallback(
		  SafeWaitChainSessionHandle WctHandle,
		  IntPtr Context,
		  UInt32 CallbackStatus,
		  ref UInt32 NodeCount,
		  WaitChanNodeInfo.Native* NodeInfoArray,
		  [MarshalAs(UnmanagedType.Bool)] ref bool IsCycle
		);

		[DllImport("Advapi32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern SafeWaitChainSessionHandle OpenThreadWaitChainSession(UInt32 flags, Pwaitchaincallback callback);

		enum WCT_SESSION_OPEN_FLAGS : uint { WCT_SYNC_OPEN_FLAG = 0, WCT_ASYNC_OPEN_FLAG = 1 };
	}
}
