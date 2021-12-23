using System;
using System.Runtime.InteropServices;
using Henke37.Win32.Debug.Info;
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
						return new ExitThreadEvent(processId, threadId, exitThreadDebugInfo.ExitCode);
					case DebugCode.ExitProcess:
						return new ExitProcessEvent(processId, threadId, exitProcessDebugInfo.ExitCode);
					case DebugCode.LoadDll:
						return new LoadDllEvent(processId, threadId,
							new SafeFileObjectHandle(loadDllDebugInfo.fileHandle),
							loadDllDebugInfo.imageBase
							);
					case DebugCode.UnloadDll:
						return new UnloadDllEvent(processId, threadId, unloadDllDebugInfo.loadBase);
					case DebugCode.OutputDebugString:
						return new OutputDebugStringEvent(processId, threadId, debugOutputStringInfo.dataAddress, debugOutputStringInfo.isUnicode!=0);
					case DebugCode.RIP:
						return new RIPEvent(processId, threadId, ripDebugInfo.error, ripDebugInfo.type);

				}
				throw new NotImplementedException();
			}

		}
	}
}
