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
	}
}
