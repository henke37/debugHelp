using Henke37.Win32.SafeHandles;
using System;

namespace Henke37.Win32.Debug.Event {
	internal class CreateProcessEvent : DebugEvent {
		private SafeProcessHandle safeProcessHandle;
		private SafeThreadHandle safeThreadHandle;
		private IntPtr startAddress;
		private IntPtr localBase;
		private SafeFileObjectHandle safeFileObjectHandle;
		private IntPtr imageBase;

		public CreateProcessEvent(
			uint processId,
			uint threadId,
			SafeProcessHandle safeProcessHandle,
			SafeThreadHandle safeThreadHandle,
			IntPtr startAddress,
			IntPtr localBase,
			SafeFileObjectHandle safeFileObjectHandle,
			IntPtr imageBase) : base(
			processId,
			threadId) {
			this.safeProcessHandle = safeProcessHandle;
			this.safeThreadHandle = safeThreadHandle;
			this.startAddress = startAddress;
			this.localBase = localBase;
			this.safeFileObjectHandle = safeFileObjectHandle;
			this.imageBase = imageBase;
		}
	}
}