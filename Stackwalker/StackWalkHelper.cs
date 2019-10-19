using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dia2Lib;

namespace Stackwalker {
	internal class StackWalkHelper : FixedIDiaStackWalkHelper {
		public void addressForVA(ulong va, ref uint pISect, ref uint pOffset) {
			throw new NotImplementedException();
		}

		public void frameForVA(ulong va, ref IDiaFrameData ppFrame) {
			throw new NotImplementedException();
		}

		public void functionFragmentsForVA(ulong vaFunc, uint cbFunc, uint cFragments, ref ulong pVaFragment, ref uint pLenFragment) {
			throw new NotImplementedException();
		}

		public void imageForVA(ulong vaContext, ref ulong pvaImageStart) {
			throw new NotImplementedException();
		}

		public void numberOfFunctionFragmentsForVA(ulong vaFunc, uint cbFunc, ref uint pNumFragments) {
			throw new NotImplementedException();
		}

		public void pdataForVA(ulong va, uint cbData, ref uint pcbData, ref byte pbData) {
			throw new NotImplementedException();
		}

		public void readMemory(MemoryTypeEnum type, ulong va, uint cbData, ref uint pcbData, ref byte pbData) {
			throw new NotImplementedException();
		}

		public ulong registerValue_get(uint index) {
			throw new NotImplementedException();
		}

		public void registerValue_set(uint index, ulong value) {
			throw new NotImplementedException();
		}

		public void searchForReturnAddress(IDiaFrameData frame, ref ulong returnAddress) {
			throw new NotImplementedException();
		}

		public void searchForReturnAddressStart(IDiaFrameData frame, ulong startAddress, ref ulong returnAddress) {
			throw new NotImplementedException();
		}

		public void symbolForVA(ulong va, ref IDiaSymbol ppSymbol) {
			throw new NotImplementedException();
		}
	}
}
