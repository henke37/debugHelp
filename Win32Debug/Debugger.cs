using Henke37.Win32.Debug.Event;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Debug {
	public class Debugger {

		private static readonly object instanceLock= new object();
		private static Debugger instance;

		private Debugger() { }

		public static Debugger GetInstance() {
			lock(instanceLock) {
				if(instance != null) return instance;
				return instance = new Debugger();
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
			throw new NotImplementedException();
		}

		private void StopEventListenerThread() {
			throw new NotImplementedException();
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
	}
}
