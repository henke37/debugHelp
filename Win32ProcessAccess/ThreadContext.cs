﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
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

			bool success = GetThreadContext(handle, &native);
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

			bool success = SetThreadContext(handle, &native);
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


		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static unsafe extern bool GetThreadContext(SafeThreadHandle handle, Native* ctx);

		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		internal static unsafe extern bool SetThreadContext(SafeThreadHandle handle, Native* ctx);
	}


}