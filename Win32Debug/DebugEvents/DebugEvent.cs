using System;
using System.Runtime.InteropServices;
using Henke37.Win32.SafeHandles;

namespace Henke37.Win32.Debug.Event {
	public abstract class DebugEvent {

		public UInt32 processId;
		public UInt32 threadId;

		protected DebugEvent(UInt32 processId, UInt32 threadId) {
			this.processId = processId;
			this.threadId = threadId;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal class Native {
			[FieldOffset(0)]
			internal DebugCode debugCode;
			[FieldOffset(4)]
			internal UInt32 processId;
			[FieldOffset(8)]
			internal UInt32 threadId;

			[FieldOffset(12)]
			internal ExceptionDebugInfo exceptionDebugInfo;
			[FieldOffset(12)]
			internal CreateThreadDebugInfo createThreadDebugInfo;
			[FieldOffset(12)]
			internal CreateProcessDebugInfo createProcessDebugInfo;
			[FieldOffset(12)]
			internal ExitThreadDebugInfo exitThreadDebugInfo;
			[FieldOffset(12)]
			internal ExitProcessDebugInfo exitProcessDebugInfo;
			[FieldOffset(12)]
			internal LoadDllDebugInfo loadDllDebugInfo;
			[FieldOffset(12)]
			internal UnloadDllDebugInfo unloadDllDebugInfo;
			[FieldOffset(12)]
			internal DebugOutputStringInfo debugOutputStringInfo;
			[FieldOffset(12)]
			internal RIPDebugInfo ripDebugInfo;

			internal DebugEvent AsManaged() {
				switch(debugCode) {
					case DebugCode.Exception:
						throw new NotImplementedException();
						break;
					case DebugCode.CreateThread:
						return new CreateThreadEvent(
							processId,
							threadId,
							new SafeThreadHandle(createThreadDebugInfo.threadHandle),
							createThreadDebugInfo.startAddress,
							createThreadDebugInfo.localBase);
					case DebugCode.CreateProcess:
						return new CreateProcessEvent(processId,
							threadId,
							new SafeProcessHandle(createProcessDebugInfo.processHandle),
							new SafeThreadHandle(createProcessDebugInfo.threadHandle),
							createProcessDebugInfo.startAddress,
							createProcessDebugInfo.localBase,
							new SafeFileObjectHandle(createProcessDebugInfo.fileHandle),
							createProcessDebugInfo.imageBase
							);
					case DebugCode.ExitThread:
						throw new NotImplementedException();
						break;
					case DebugCode.ExitProcess:
						throw new NotImplementedException();
						break;
					case DebugCode.LoadDll:
						throw new NotImplementedException();
						break;
					case DebugCode.UnloadDll:
						throw new NotImplementedException();
						break;
					case DebugCode.OutputDebugString:
						throw new NotImplementedException();
						break;
					case DebugCode.RIP:
						throw new NotImplementedException();
						break;

				}
				throw new NotImplementedException();
			}

		}
	}
}
