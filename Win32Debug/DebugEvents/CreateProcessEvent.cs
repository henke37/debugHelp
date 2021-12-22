using Henke37.Win32.Processes;
using Henke37.Win32.SafeHandles;
using Henke37.Win32.Threads;
using Microsoft.Win32.SafeHandles;
using System;
using SafeProcessHandle = Henke37.Win32.SafeHandles.SafeProcessHandle;

namespace Henke37.Win32.Debug.Event {
	public class CreateProcessEvent : DebugEvent {
		private SafeProcessHandle safeProcessHandle;
		private SafeThreadHandle safeThreadHandle;
		private IntPtr startAddress;
		private IntPtr localBase;
		private SafeFileObjectHandle safeFileObjectHandle;
		private IntPtr imageBase;

		public NativeProcess Process => new NativeProcess(safeProcessHandle);
		public NativeThread FirstThread => new NativeThread(safeThreadHandle);
		public SafeFileHandle FileHandle => safeFileObjectHandle.AsSafeFileHandle();

		internal CreateProcessEvent(
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