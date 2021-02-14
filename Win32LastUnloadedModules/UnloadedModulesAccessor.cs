using Henke37.DebugHelp;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Henke37.Win32.LastUnloadedModules {
	public class UnloadedModulesAccessor {

		private static IntPtr ElementSizePtr;
		private static IntPtr ElementCountPtr;
		private static IntPtr EventTracePtr;

		private ProcessMemoryAccessor processMem;

		static UnloadedModulesAccessor() {
			LoadPointers();
		}

		private static void LoadPointers() {
			RtlGetUnloadEventTraceEx(out ElementSizePtr, out ElementCountPtr, out EventTracePtr);
		}

		public UnloadedModulesAccessor(ProcessMemoryAccessor processMem) {
			this.processMem = processMem;
			CheckStructSize();
		}

		public UnloadEventTrace[] ReadUnloadedModules() {
			var count = processMem.ReadUInt32(ElementCountPtr);
			var nArr = processMem.ReadStructArr<UnloadEventTrace.Native>(EventTracePtr, count);
			var oArr = new UnloadEventTrace[count];

			for(var i=0;i<count;++i) {
				oArr[i] = nArr[i].AsManaged();
			}

			return oArr;
		}

		private unsafe void CheckStructSize() {
			var currentSize = processMem.ReadUInt32(ElementSizePtr);
			var structSize = sizeof(UnloadEventTrace.Native);
			Debug.Assert(structSize == currentSize);
		}

		[DllImport("Ntdll.dll", SetLastError = true)]
		static extern void RtlGetUnloadEventTraceEx(
		  out IntPtr ElementSize,
		  out IntPtr ElementCount,
		  out IntPtr EventTrace
		);
	}
}
