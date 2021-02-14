using Henke37.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Threads {
	public abstract class ThreadContext {
	}

	public class ThreadContext32 : ThreadContext {

		public UInt32 SegDs;
		public UInt32 SegCs;
		public UInt32 SegSs;
		public UInt32 SegGs;
		public UInt32 SegFs;
		public UInt32 SegEs;

		public UInt32 Edi;
		public UInt32 Esi;
		public UInt32 Eax;
		public UInt32 Ebx;
		public UInt32 Edx;
		public UInt32 Ecx;
		public UInt32 Esp;
		public UInt32 Ebp;
		public UInt32 Eip;
		public EFlags EFlags;

		internal unsafe void ReadFromHandle(SafeThreadHandle handle) {
			Native native;

			native.ContextFlags = (UInt32)(ContextFlags.i386 | ContextFlags.Integer | ContextFlags.Control | ContextFlags.Segments);

#if x86
			bool success = GetThreadContext(handle, &native);
#elif x64
			bool success = Wow64GetThreadContext(handle, &native);
#endif
			if(!success) throw new Win32Exception();

			Eax = native.Eax;
			Ebx = native.Ebx;
			Ecx = native.Ecx;
			Edx = native.Edx;
			Edi = native.Edi;
			Esi = native.Esi;
			Ebp = native.Ebp;
			Eip = native.Eip;
			EFlags = (EFlags)native.EFlags;
			SegDs = native.SegDs;
			SegCs = native.SegCs;
			SegSs = native.SegSs;
			SegGs = native.SegGs;
			SegFs = native.SegFs;
			SegEs = native.SegEs;
		}

		internal unsafe void WriteToHandle(SafeThreadHandle handle) {
			Native native;

			native.ContextFlags = (UInt32)(ContextFlags.i386 | ContextFlags.Integer | ContextFlags.Control | ContextFlags.Segments);

			native.Eax = Eax;
			native.Ebx = Ebx;
			native.Ecx = Ecx;
			native.Edx = Edx;
			native.Edi = Edi;
			native.Esi = Esi;
			native.Ebp = Ebp;
			native.Eip = Eip;
			native.EFlags = (UInt32)EFlags;
			native.SegDs = SegDs;
			native.SegCs = SegCs;
			native.SegSs = SegSs;
			native.SegGs = SegGs;
			native.SegFs = SegFs;
			native.SegEs = SegEs;

#if x86
			bool success = SetThreadContext(handle, &native);
#elif x64
			bool success = Wow64SetThreadContext(handle, &native);
#endif
			if(!success) throw new Win32Exception();
		}

		internal unsafe struct Native {
			internal UInt32 ContextFlags;
			internal UInt32 Dr0;
			internal UInt32 Dr1;
			internal UInt32 Dr2;
			internal UInt32 Dr3;
			internal UInt32 Dr6;
			internal UInt32 Dr7;
			internal FLOATING_SAVE_AREA FloatSave;
			internal UInt32 SegGs;
			internal UInt32 SegFs;
			internal UInt32 SegEs;
			internal UInt32 SegDs;
			internal UInt32 Edi;
			internal UInt32 Esi;
			internal UInt32 Ebx;
			internal UInt32 Edx;
			internal UInt32 Ecx;
			internal UInt32 Eax;
			internal UInt32 Ebp;
			internal UInt32 Eip;
			internal UInt32 SegCs;
			internal UInt32 EFlags;
			internal UInt32 Esp;
			internal UInt32 SegSs;
			internal fixed byte ExtendedRegisters[512];
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

#if x86
		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static unsafe extern bool GetThreadContext(SafeThreadHandle handle, Native* ctx);

		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static unsafe extern bool SetThreadContext(SafeThreadHandle handle, Native* ctx);
#elif x64
		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static unsafe extern bool Wow64GetThreadContext(SafeThreadHandle handle, Native* ctx);

		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static unsafe extern bool Wow64SetThreadContext(SafeThreadHandle handle, Native* ctx);
#endif
	}

#if x64
	public class ThreadContext64 : ThreadContext {

		internal void ReadFromHandle(SafeThreadHandle handle) {
			throw new NotImplementedException();
		}

		internal void WriteToHandle(SafeThreadHandle handle) {
			throw new NotImplementedException();
		}

		internal unsafe struct Native {

			[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
			internal static unsafe extern bool GetThreadContext(SafeThreadHandle handle, Native* ctx);

			[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
			internal static unsafe extern bool SetThreadContext(SafeThreadHandle handle, Native* ctx);
		}
	}
#endif


#if arm64
	public class ThreadContextArm64 : ThreadContext {

		public UInt32 ContextFlags;
		public UInt32 Cpsr;
		public UInt64 X0;
		public UInt64 X1;
		public UInt64 X2;
		public UInt64 X3;
		public UInt64 X4;
		public UInt64 X5;
		public UInt64 X6;
		public UInt64 X7;
		public UInt64 X8;
		public UInt64 X9;
		public UInt64 X10;
		public UInt64 X11;
		public UInt64 X12;
		public UInt64 X13;
		public UInt64 X14;
		public UInt64 X15;
		public UInt64 X16;
		public UInt64 X17;
		public UInt64 X18;
		public UInt64 X19;
		public UInt64 X20;
		public UInt64 X21;
		public UInt64 X22;
		public UInt64 X23;
		public UInt64 X24;
		public UInt64 X25;
		public UInt64 X26;
		public UInt64 X27;
		public UInt64 X28;
		public UInt64 Fp;
		public UInt64 Lr;
		public UInt64 Sp;
		public UInt64 Pc;

		internal void ReadFromHandle(SafeThreadHandle handle) {
			throw new NotImplementedException();
		}

		internal void WriteToHandle(SafeThreadHandle handle) {
			throw new NotImplementedException();
		}

		internal unsafe struct Native {
			UInt32 ContextFlags;
			UInt32 Cpsr;
			UInt64 X0;
			UInt64 X1;
			UInt64 X2;
			UInt64 X3;
			UInt64 X4;
			UInt64 X5;
			UInt64 X6;
			UInt64 X7;
			UInt64 X8;
			UInt64 X9;
			UInt64 X10;
			UInt64 X11;
			UInt64 X12;
			UInt64 X13;
			UInt64 X14;
			UInt64 X15;
			UInt64 X16;
			UInt64 X17;
			UInt64 X18;
			UInt64 X19;
			UInt64 X20;
			UInt64 X21;
			UInt64 X22;
			UInt64 X23;
			UInt64 X24;
			UInt64 X25;
			UInt64 X26;
			UInt64 X27;
			UInt64 X28;
			UInt64 Fp;
			UInt64 Lr;
			UInt64 Sp;
			UInt64 Pc;

			[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
			internal static unsafe extern bool GetThreadContext(SafeThreadHandle handle, Native* ctx);

			[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
			internal static unsafe extern bool SetThreadContext(SafeThreadHandle handle, Native* ctx);
		}
	}
#endif
}