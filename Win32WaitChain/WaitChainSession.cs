using System;
using System.Runtime.InteropServices;

namespace Win32WaitChain {
	public class WaitChainSession {
		SafeWaitChainSessionHandle handle;

		public WaitChainSession() {

		}


		internal unsafe delegate void Pwaitchaincallback(
		  SafeWaitChainSessionHandle WctHandle,
		  IntPtr Context,
		  UInt32 CallbackStatus,
		  out UInt32 NodeCount,
		  WaitChanNodeInfo.Native *NodeInfoArray,
		  [MarshalAs(UnmanagedType.Bool)] out bool IsCycle
		);

		[DllImport("Advapi32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern SafeWaitChainSessionHandle OpenThreadWaitChainSession(UInt32 flags, Pwaitchaincallback callback);


		[DllImport("Advapi32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool GetThreadWaitChain(SafeWaitChainSessionHandle handle, void* context, UInt32 flags, UInt32 threadId, ref UInt32 nodeCount,
		  WaitChanNodeInfo.Native* NodeInfoArray,
		  [MarshalAs(UnmanagedType.Bool)] out bool IsCycle);
	}
}
