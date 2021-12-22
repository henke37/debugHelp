using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.Debug {
	public abstract class DebugEvent {

		public UInt32 processId;
		public UInt32 threadId;

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
						throw new NotImplementedException();
						break;
					case DebugCode.CreateProcess:
						throw new NotImplementedException();
						break;
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
