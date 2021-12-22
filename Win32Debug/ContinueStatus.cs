using System;

namespace Henke37.Win32.Debug {
	public enum ContinueStatus : UInt32 {
		Continue = 0x00010002,
		ExceptionNotHandled = 0x80010001,
		ReplyLater = 0x40010001
	}
}
