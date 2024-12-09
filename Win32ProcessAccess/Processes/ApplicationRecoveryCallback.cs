using System;

namespace Henke37.Win32.Processes {
	public class ApplicationRecoveryCallback {
		public UIntPtr callback;
		public UIntPtr pvoidParam;
		public UInt32 pingInterval;
		public UInt32 flags;
	}
}