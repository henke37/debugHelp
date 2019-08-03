using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
	public abstract class ThreadContext {
	}

	public class ThreadContext32 : ThreadContext {

		public ValidThreadContextFields valid;

		internal unsafe void ReadFromHandle(SafeThreadHandle handle) {
			Native native;

			bool success=GetThreadContext(handle, &native);
			if(!success) throw new Win32Exception();

			throw new NotImplementedException();
		}

		internal unsafe void WriteToHandle(SafeThreadHandle handle) {
			Native native;

			throw new NotImplementedException();

			bool success = SetThreadContext(handle, &native);
			if(!success) throw new Win32Exception();

		}

		[Flags]
		public enum ValidThreadContextFields {
			None=0,
			Basic=1,
			FPU=2
		}

		internal unsafe struct Native {
			UInt32 ContextFlags;
			UInt32 Dr0;
			UInt32 Dr1;
			UInt32 Dr2;
			UInt32 Dr3;
			UInt32 Dr6;
			UInt32 Dr7;
			FLOATING_SAVE_AREA FloatSave;
			UInt32 SegGs;
			UInt32 SegFs;
			UInt32 SegEs;
			UInt32 SegDs;
			UInt32 Edi;
			UInt32 Esi;
			UInt32 Ebx;
			UInt32 Edx;
			UInt32 Ecx;
			UInt32 Eax;
			UInt32 Ebp;
			UInt32 Eip;
			UInt32 SegCs;
			UInt32 EFlags;
			UInt32 Esp;
			UInt32 SegSs;
			fixed byte ExtendedRegisters[512];
		}

		internal unsafe struct FLOATING_SAVE_AREA {
			UInt32 ControlWord;
			UInt32 StatusWord;
			UInt32 TagWord;
			UInt32 ErrorOffset;
			UInt32 ErrorSelector;
			UInt32 DataOffset;
			UInt32 DataSelector;
			fixed byte RegisterArea[80];
			UInt32 Cr0NpxState;
		}


		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static unsafe extern bool GetThreadContext(SafeThreadHandle handle, Native* ctx);

		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static unsafe extern bool SetThreadContext(SafeThreadHandle handle, Native* ctx);
	}

	
}