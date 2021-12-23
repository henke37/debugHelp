using System;

namespace Henke37.Win32.Debug {
	public class ExceptionRecord {
		public ExceptionCode ExceptionCode;
		public ExceptionFlag ExceptionFlags;
		public ExceptionRecord Other;
		public IntPtr ExceptionAddress;
		public uint[] ExceptionInfo;
	}
}