using Henke37.Win32.SafeHandles;
using Henke37.Win32.Threads;
using System;

namespace Henke37.Win32.Debug.Event {
	public class CreateThreadEvent : DebugEvent {
		private SafeThreadHandle safeThreadHandle;
		public IntPtr startAddress;
		public IntPtr localBase;

		public NativeThread NativeThread => new NativeThread(safeThreadHandle);

		internal CreateThreadEvent(uint processId, uint threadId, SafeThreadHandle safeThreadHandle, IntPtr startAddress, IntPtr localBase) : base(processId, threadId) {
			this.safeThreadHandle = safeThreadHandle;
			this.startAddress = startAddress;
			this.localBase = localBase;
		}
	}
}