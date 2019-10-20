using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIA;
using Henke37.DebugHelp;
using Henke37.DebugHelp.Win32;

namespace Stackwalker {
	internal class StackWalkHelper : IDiaStackWalkHelper {

		private ThreadContext Context;
		private NativeThread Thread;
		private ProcessMemoryAccessor MemoryAccessor;

		private ThreadContext32 context;

		internal StackWalkHelper(NativeThread thread, ProcessMemoryAccessor memoryAccessor) {
			Thread = thread;
			MemoryAccessor = memoryAccessor;
		}

		internal void InitializeForWalk() {
			Context=Thread.GetContext();
		}

		public int readMemory(MemoryTypeEnum type, ulong va, uint cbData, out uint pcbData, byte[] pbData) {
			throw new NotImplementedException();
		}

		public int searchForReturnAddress(IDiaFrameData frame, out ulong returnAddress) {
			throw new NotImplementedException();
		}

		public int searchForReturnAddressStart(IDiaFrameData frame, ulong startAddress, out ulong returnAddress) {
			throw new NotImplementedException();
		}

		public int frameForVA(ulong va, out IDiaFrameData ppFrame) {
			throw new NotImplementedException();
		}

		public int symbolForVA(ulong va, out IDiaSymbol ppSymbol) {
			throw new NotImplementedException();
		}

		public int pdataForVA(ulong va, uint cbData, out uint pcbData, byte[] pbData) {
			throw new NotImplementedException();
		}

		public int imageForVA(ulong vaContext, out ulong pvaImageStart) {
			throw new NotImplementedException();
		}

		public int addressForVA(ulong va, out uint pISect, out uint pOffset) {
			throw new NotImplementedException();
		}

		public int numberOfFunctionFragmentsForVA(ulong vaFunc, uint cbFunc, out uint pNumFragments) {
			throw new NotImplementedException();
		}

		public int functionFragmentsForVA(ulong vaFunc, uint cbFunc, uint cFragments, out ulong pVaFragment, out uint pLenFragment) {
			throw new NotImplementedException();
		}

		public ulong registerValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}
