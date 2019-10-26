using System;
using DIA;
using Henke37.DebugHelp;
using Henke37.DebugHelp.PdbAccess;
using Henke37.DebugHelp.Win32;

namespace Stackwalker {
	internal class StackWalkHelper : IDiaStackWalkHelper {

		private ThreadContext32 Context;
		private NativeThread Thread;
		private ProcessMemoryAccessor MemoryAccessor;
		private SymbolResolver Resolver;

		private const int S_OK=0;
		private const int E_FAIL = unchecked((int)0x80004005);
		private const int E_NOTIMPL = unchecked((int)0x80004001);

		internal StackWalkHelper(NativeThread thread, ProcessMemoryAccessor memoryAccessor, SymbolResolver resolver) {
			Thread = thread;
			MemoryAccessor = memoryAccessor;
			Resolver = resolver;
		}

		internal void InitializeForWalk() {
			Context=Thread.GetContext();
		}

		int IDiaStackWalkHelper.readMemory(MemoryTypeEnum type, ulong va, uint cbData, out uint pcbData, byte[] pbData) {
			MemoryAccessor.ReadBytes((IntPtr)va, cbData, pbData);
			pcbData = cbData;
			return S_OK;
		}

		int IDiaStackWalkHelper.searchForReturnAddress(IDiaFrameData frame, out ulong returnAddress) {
			returnAddress = 0;
			return E_NOTIMPL;
		}

		int IDiaStackWalkHelper.searchForReturnAddressStart(IDiaFrameData frame, ulong startAddress, out ulong returnAddress) {
			returnAddress = 0;
			return E_NOTIMPL;
		}

		int IDiaStackWalkHelper.frameForVA(ulong va, out IDiaFrameData ppFrame) {
			ppFrame = null;
			return E_NOTIMPL;
		}

		int IDiaStackWalkHelper.symbolForVA(ulong va, out IDiaSymbol ppSymbol) {
			ppSymbol = Resolver.FindFunctionAtAddr((IntPtr)va);
			return S_OK;
		}

		int IDiaStackWalkHelper.pdataForVA(ulong va, uint cbData, out uint pcbData, byte[] pbData) {
			pcbData = 0;
			return E_NOTIMPL;
		}

		int IDiaStackWalkHelper.imageForVA(ulong vaContext, out ulong pvaImageStart) {
			pvaImageStart = 0;
			return E_NOTIMPL;
		}

		int IDiaStackWalkHelper.addressForVA(ulong va, out uint pISect, out uint pOffset) {
			Resolver.AddressForVirtualAddress((IntPtr)va, out pISect, out pOffset);
			return S_OK;
		}

		int IDiaStackWalkHelper.numberOfFunctionFragmentsForVA(ulong vaFunc, uint cbFunc, out uint pNumFragments) {
			pNumFragments = 0;
			return E_NOTIMPL;
		}

		int IDiaStackWalkHelper.functionFragmentsForVA(ulong vaFunc, uint cbFunc, uint cFragments, out ulong pVaFragment, out uint pLenFragment) {
			pVaFragment = 0;
			pLenFragment = 0;
			return E_NOTIMPL;
		}


		int IDiaStackWalkHelper.registerValue_get(CV_HREG_e Index, out ulong retVal) {
			switch(Index) {
				case CV_HREG_e.CV_REG_EAX: retVal = Context.Eax; return S_OK;
				case CV_HREG_e.CV_REG_ECX: retVal = Context.Ecx; return S_OK;
				case CV_HREG_e.CV_REG_EDX: retVal = Context.Edx; return S_OK;
				case CV_HREG_e.CV_REG_EBX: retVal = Context.Ebx ; return S_OK;
				case CV_HREG_e.CV_REG_ESP: retVal = Context.Esp; return S_OK;
				case CV_HREG_e.CV_REG_EBP: retVal = Context.Ebp; return S_OK;
				case CV_HREG_e.CV_REG_ESI: retVal = Context.Esi; return S_OK;
				case CV_HREG_e.CV_REG_EDI: retVal = Context.Edi; return S_OK;
				case CV_HREG_e.CV_REG_ES: retVal = Context.SegEs; return S_OK;
				case CV_HREG_e.CV_REG_CS: retVal = Context.SegCs; return S_OK;
				case CV_HREG_e.CV_REG_SS: retVal = Context.SegSs; return S_OK;
				case CV_HREG_e.CV_REG_DS: retVal = Context.SegDs; return S_OK;
				case CV_HREG_e.CV_REG_FS: retVal = Context.SegFs; return S_OK;
				case CV_HREG_e.CV_REG_GS: retVal = Context.SegGs; return S_OK;
				case CV_HREG_e.CV_REG_IP: retVal = Context.Eip; return S_OK;
				case CV_HREG_e.CV_REG_EFLAGS: retVal = (ulong)Context.EFlags; return S_OK;
				default:
					retVal = 0;
					return E_FAIL;
			}
		}

		int IDiaStackWalkHelper.registerValue_put(CV_HREG_e Index, ulong newVal) {
			switch(Index) {
				case CV_HREG_e.CV_REG_EAX: Context.Eax = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_ECX: Context.Ecx = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_EDX: Context.Edx = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_EBX: Context.Ebx = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_ESP: Context.Esp = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_EBP: Context.Ebp = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_ESI: Context.Esi = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_EDI: Context.Edi = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_ES: Context.SegEs = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_CS: Context.SegCs = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_SS: Context.SegSs = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_DS: Context.SegDs = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_FS: Context.SegFs = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_GS: Context.SegGs = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_IP: Context.Eip = (uint)newVal; return S_OK;
				case CV_HREG_e.CV_REG_EFLAGS: Context.EFlags = (EFlags)newVal; return S_OK;
			}
			return E_FAIL;
		}
	}
}
