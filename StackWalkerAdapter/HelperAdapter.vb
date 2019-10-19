Imports Dia2Lib

Namespace Stackwalker
	Friend Class HelperAdapter
		Implements IDiaStackWalkHelper

		Private helper As FixedIDiaStackWalkHelper

		Public Sub New(helper As FixedIDiaStackWalkHelper)
			Me.helper = helper
		End Sub

		Private Sub IDiaStackWalkHelper_readMemory(type As MemoryTypeEnum, va As ULong, cbData As UInteger, ByRef pcbData As UInteger, ByRef pbData As Byte) Implements IDiaStackWalkHelper.readMemory
			helper.readMemory(type, va, cbData, pcbData, pbData)
		End Sub

		Private Sub IDiaStackWalkHelper_searchForReturnAddress(frame As IDiaFrameData, ByRef returnAddress As ULong) Implements IDiaStackWalkHelper.searchForReturnAddress
			helper.searchForReturnAddress(frame, returnAddress)
		End Sub

		Private Sub IDiaStackWalkHelper_searchForReturnAddressStart(frame As IDiaFrameData, startAddress As ULong, ByRef returnAddress As ULong) Implements IDiaStackWalkHelper.searchForReturnAddressStart
			helper.searchForReturnAddressStart(frame, startAddress, returnAddress)
		End Sub

		Private Sub IDiaStackWalkHelper_frameForVA(va As ULong, ByRef ppFrame As IDiaFrameData) Implements IDiaStackWalkHelper.frameForVA
			helper.frameForVA(va, ppFrame)
		End Sub

		Private Sub IDiaStackWalkHelper_symbolForVA(va As ULong, ByRef ppSymbol As IDiaSymbol) Implements IDiaStackWalkHelper.symbolForVA
			helper.symbolForVA(va, ppSymbol)
		End Sub

		Private Sub IDiaStackWalkHelper_pdataForVA(va As ULong, cbData As UInteger, ByRef pcbData As UInteger, ByRef pbData As Byte) Implements IDiaStackWalkHelper.pdataForVA
			helper.pdataForVA(va, cbData, pcbData, pbData)
		End Sub

		Private Sub IDiaStackWalkHelper_imageForVA(vaContext As ULong, ByRef pvaImageStart As ULong) Implements IDiaStackWalkHelper.imageForVA
			helper.imageForVA(vaContext, pvaImageStart)
		End Sub

		Private Sub IDiaStackWalkHelper_addressForVA(va As ULong, ByRef pISect As UInteger, ByRef pOffset As UInteger) Implements IDiaStackWalkHelper.addressForVA
			helper.addressForVA(va, pISect, pOffset)
		End Sub

		Private Sub IDiaStackWalkHelper_numberOfFunctionFragmentsForVA(vaFunc As ULong, cbFunc As UInteger, ByRef pNumFragments As UInteger) Implements IDiaStackWalkHelper.numberOfFunctionFragmentsForVA
			helper.numberOfFunctionFragmentsForVA(vaFunc, cbFunc, pNumFragments)
		End Sub

		Private Sub IDiaStackWalkHelper_functionFragmentsForVA(vaFunc As ULong, cbFunc As UInteger, cFragments As UInteger, ByRef pVaFragment As ULong, ByRef pLenFragment As UInteger) Implements IDiaStackWalkHelper.functionFragmentsForVA
			helper.functionFragmentsForVA(vaFunc, cbFunc, cFragments, pVaFragment, pLenFragment)
		End Sub

		Public Property registerValue(index As UInteger) As ULong Implements IDiaStackWalkHelper.registerValue
			Get
				Return helper.registerValue_get(index)
			End Get
			Set(value As ULong)
				helper.registerValue_set(index, value)
			End Set
		End Property
	End Class
End Namespace
