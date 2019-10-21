using System;
using DIA;
using Henke37.DebugHelp;
using Henke37.DebugHelp.Win32;

namespace Stackwalker {
	internal class StackWalkHelper : IDiaStackWalkHelper {

		private ThreadContext Context;
		private NativeThread Thread;
		private ProcessMemoryAccessor MemoryAccessor;

		private ThreadContext32 context;

		private const int S_OK=0;
		private const int E_NOTIMPL = unchecked((int)0x80004001);

		internal StackWalkHelper(NativeThread thread, ProcessMemoryAccessor memoryAccessor) {
			Thread = thread;
			MemoryAccessor = memoryAccessor;
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
			ppSymbol = null;
			return E_NOTIMPL;
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
			pISect = 0;
			pOffset = 0;
			return E_NOTIMPL;
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
				case 17: return context.Eax;
				case 18: return context.Ecx;
				case 19: return context.Edx;
				case 20: return context.Ebx;
				case 21: return context.Esp;
				case 22: return context.Ebp;
				case 23: return context.Esi;
				case 24: return context.Edi;
				case 25: return context.SegEs;
				case 26: return context.SegCs;
				case 27: return context.SegSs;
				case 28: return context.SegDs;
				case 29: return context.SegFs;
				case 30: return context.SegGs;
				case 31: return context.Eip;
				case 32: return (ulong)context.EFlags;
				default:
					throw new NotSupportedException();
			}
		}

		private void SetRegisterValue(int index, ulong value) {
			switch(index) {
				case 17: context.Eax = (uint)value; return;
				case 18: context.Ecx = (uint)value; return;
				case 19: context.Edx = (uint)value; return;
				case 20: context.Ebx = (uint)value; return;
				case 21: context.Esp = (uint)value; return;
				case 22: context.Ebp = (uint)value; return;
				case 23: context.Esi = (uint)value; return;
				case 24: context.Edi = (uint)value; return;
				case 25: context.SegEs = (uint)value; return;
				case 26: context.SegCs = (uint)value; return;
				case 27: context.SegSs = (uint)value; return;
				case 28: context.SegDs = (uint)value; return;
				case 29: context.SegFs = (uint)value; return;
				case 30: context.SegGs = (uint)value; return;
				case 31: context.Eip = (uint)value; return;
				case 32: context.EFlags = (EFlags)value; return;
			}
			throw new NotSupportedException();
		}
	}
}
