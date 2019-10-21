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

		ulong IDiaStackWalkHelper.registerValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		private ulong GetRegisterValue(int index) {
			switch(index) {
				case 17: return Context.Eax;
				case 18: return Context.Ecx;
				case 19: return Context.Edx;
				case 20: return Context.Ebx;
				case 21: return Context.Esp;
				case 22: return Context.Ebp;
				case 23: return Context.Esi;
				case 24: return Context.Edi;
				case 25: return Context.SegEs;
				case 26: return Context.SegCs;
				case 27: return Context.SegSs;
				case 28: return Context.SegDs;
				case 29: return Context.SegFs;
				case 30: return Context.SegGs;
				case 31: return Context.Eip;
				case 32: return (ulong)Context.EFlags;
				default:
					throw new NotSupportedException();
			}
		}

		private void SetRegisterValue(int index, ulong value) {
			switch(index) {
				case 17: Context.Eax = (uint)value; return;
				case 18: Context.Ecx = (uint)value; return;
				case 19: Context.Edx = (uint)value; return;
				case 20: Context.Ebx = (uint)value; return;
				case 21: Context.Esp = (uint)value; return;
				case 22: Context.Ebp = (uint)value; return;
				case 23: Context.Esi = (uint)value; return;
				case 24: Context.Edi = (uint)value; return;
				case 25: Context.SegEs = (uint)value; return;
				case 26: Context.SegCs = (uint)value; return;
				case 27: Context.SegSs = (uint)value; return;
				case 28: Context.SegDs = (uint)value; return;
				case 29: Context.SegFs = (uint)value; return;
				case 30: Context.SegGs = (uint)value; return;
				case 31: Context.Eip = (uint)value; return;
				case 32: Context.EFlags = (EFlags)value; return;
			}
			throw new NotSupportedException();
		}
	}
}
