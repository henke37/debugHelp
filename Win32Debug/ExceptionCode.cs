namespace Henke37.Win32.Debug {
	public enum ExceptionCode : uint {
		AccessViolation = 0xC0000005,
		ArrayBoundsExceeded = 0xC000008C,
		Breakpoint = 0x80000003,
		DatatypeMisalignment = 0x80000002,
		FloatDenormalOperand = 0xC000008D,
		FloatDivideByZero = 0xC000008E,
		FloatInexactResult = 0xC000008F,
		FloatInvalidOperation = 0xC0000090,
		FloatOverflow = 0xC0000091,
		FloatStackCheck = 0xC0000092,
		FloatUnderflow = 0xC0000093,
		GuardPage = 0x80000001,
		InvalidHandle = 0xC0000008,
		IllegalInstruction = 0xC000001D,
		InPageError = 0xC0000006,
		IntDivideByZero = 0xC0000094,
		IntOverflow = 0xC0000095,
		InvalidDisposition = 0xC0000026,
		NonContinuableException = 0xC0000025,
		PrivInstruction = 0xC0000096,
		SingleStep = 0x80000004,
		StackOverflow = 0xC00000FD
	}
}