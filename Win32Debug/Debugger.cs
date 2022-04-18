using Henke37.Win32.Debug.Event;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace Henke37.Win32.Debug {
	public class Debugger {

		private static readonly object instanceLock= new object();
		private static Debugger instance;

		public static Debugger GetInstance() {
			lock(instanceLock) {
				if(instance != null) return instance;
				return instance = new Debugger();
			}
		}

		private Thread eventWaitThread;

		private Debugger() { }

		public bool KillAttachedProcessesOnExit {
			set {
				bool success = DebugSetProcessKillOnExit(value);
				if(!success) throw new Win32Exception();
			}
		}

		public void ContinueDebugEvent(UInt32 processId, UInt32 threadId, ContinueStatus status) {
			bool success = ContinueDebugEventNative(processId, threadId, status);
			if(!success) throw new Win32Exception();
		}

		public void DebugActiveProcess(UInt32 processId) {
			bool success = DebugActiveProcessNative(processId);
			if(!success) throw new Win32Exception();
		}

		public void DebugActiveProcessStop(UInt32 processId) {
			bool success = DebugActiveProcessStopNative(processId);
			if(!success) throw new Win32Exception();
		}

		private DebugEvent WaitForDebugEvent(UInt32 timeout) {
			bool success = WaitForDebugEventEx(out var evt, timeout);
			if(!success) throw new Win32Exception();

			return evt.AsManaged();
		}

		private Action<DebugEvent> debugEventListeners;
		private const uint INFINITE=0xFFFFFFFF;

		public event Action<DebugEvent> DebugEvent {
			add {
				if(debugEventListeners!=null) {
					debugEventListeners += value;
				} else {
					debugEventListeners = value;
					StartEventListenerThread();
				}
			}
			remove {
				debugEventListeners -= value;
				if(debugEventListeners == null) {
					StopEventListenerThread();
				}
			}
		}

		private void StartEventListenerThread() {
			eventWaitThread = new Thread(ListenerThread);
		}
		private void StopEventListenerThread() {
			eventWaitThread.Abort();
		}

		private void ListenerThread() {
			for(; ; ) {
				DebugEvent evt = WaitForDebugEvent(INFINITE);
				debugEventListeners?.Invoke(evt);
			}
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "ContinueDebugEvent")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ContinueDebugEventNative(UInt32 processId, UInt32 threadId, ContinueStatus status);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "DebugActiveProcess")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DebugActiveProcessNative(UInt32 processId);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "DebugActiveProcessStop")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DebugActiveProcessStopNative(UInt32 processId);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "WaitForDebugEventEx")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WaitForDebugEventEx(out DebugEvent.Native evt, UInt32 timeout);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "DebugSetProcessKillOnExit")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DebugSetProcessKillOnExit([MarshalAs(UnmanagedType.Bool)] bool killOnExit);
	}
}
