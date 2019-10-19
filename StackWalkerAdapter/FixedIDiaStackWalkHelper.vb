Imports Dia2Lib

Friend Interface FixedIDiaStackWalkHelper
	Sub readMemory(type As MemoryTypeEnum, va As ULong, cbData As UInteger, ByRef pcbData As UInteger, ByRef pbData As Byte)

	Sub searchForReturnAddress(frame As IDiaFrameData, ByRef returnAddress As ULong)

	Sub searchForReturnAddressStart(frame As IDiaFrameData, startAddress As ULong, ByRef returnAddress As ULong)

	Sub frameForVA(va As ULong, ByRef ppFrame As IDiaFrameData)

	Sub symbolForVA(va As ULong, ByRef ppSymbol As IDiaSymbol)

	Sub pdataForVA(va As ULong, cbData As UInteger, ByRef pcbData As UInteger, ByRef pbData As Byte)

	Sub imageForVA(vaContext As ULong, ByRef pvaImageStart As ULong)

	Sub addressForVA(va As ULong, ByRef pISect As UInteger, ByRef pOffset As UInteger)

	Sub numberOfFunctionFragmentsForVA(vaFunc As ULong, cbFunc As UInteger, ByRef pNumFragments As UInteger)

	Sub functionFragmentsForVA(vaFunc As ULong, cbFunc As UInteger, cFragments As UInteger, ByRef pVaFragment As ULong, ByRef pLenFragment As UInteger)

	Function registerValue_get(index As UInteger) As ULong
	Sub registerValue_set(index As UInteger, value As ULong)
End Interface
