using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Henke37.Win32.WaitChain {
	internal unsafe class AsyncOperation : IDisposable {
		TaskCompletionSource<List<WaitChanNodeInfo>> completionSource;
		GCHandle gcHandle;

		WaitChanNodeInfo.Native* nodeArray;
		uint nodeCount;

		WaitChainSession session;

		UInt32 threadId;
		GetWaitChainFlags flags;

		private const uint ERROR_SUCCESS = 0;
		private const int ERROR_IO_PENDING = 997;

		public AsyncOperation(WaitChainSession session, GetWaitChainFlags flags, uint threadId) {
			this.session = session;
			this.threadId = threadId;
			this.flags = flags;

			completionSource = new TaskCompletionSource<List<WaitChanNodeInfo>>();
			gcHandle = GCHandle.Alloc(this, GCHandleType.Normal);

			nodeCount = 0;
			nodeArray = null;
			IssueCall();
		}

		private void IssueCall() {
			bool success = GetThreadWaitChain(session.handle, GetContext(), (uint)flags, threadId, ref nodeCount, nodeArray, out bool isCycle);
			if(!success) {
				int err = Marshal.GetLastWin32Error();
				if(err != ERROR_IO_PENDING) {
					SetException(new Win32Exception(err));
				}
			}
		}

		internal static unsafe void asyncCallback(SafeWaitChainSessionHandle WctHandle, IntPtr Context, uint CallbackStatus, ref uint NodeCount, WaitChanNodeInfo.Native* NodeInfoArray, ref bool IsCycle) {
			AsyncOperation self = (AsyncOperation)GCHandle.FromIntPtr(Context).Target;
			self.asyncCallback(CallbackStatus, ref NodeCount, NodeInfoArray, ref IsCycle);
		}

		private unsafe void asyncCallback(uint callbackStatus, ref uint nodeCount, WaitChanNodeInfo.Native* nodeInfoArray, ref bool isCycle) {
			if(callbackStatus != ERROR_SUCCESS) {
				SetException(new Win32Exception((int)callbackStatus));
				return;
			}
			throw new NotImplementedException();
		}

		internal void SetException(Exception exception) {
			completionSource.SetException(exception);
		}

		internal IntPtr GetContext() => GCHandle.ToIntPtr(gcHandle);

		internal Task<List<WaitChanNodeInfo>> GetTask() => completionSource.Task;

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				gcHandle.Free();

				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose() {
			Dispose(true);
		}
		#endregion

		[DllImport("Advapi32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool GetThreadWaitChain(
			SafeWaitChainSessionHandle handle,
			IntPtr context,
			UInt32 flags,
			UInt32 threadId,
			ref UInt32 nodeCount,
			WaitChanNodeInfo.Native* NodeInfoArray,
			[MarshalAs(UnmanagedType.Bool)] out bool IsCycle
		);
	}
}
