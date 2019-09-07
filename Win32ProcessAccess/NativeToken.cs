using Henke37.DebugHelp.Win32.SafeHandles;
using System;

namespace Henke37.DebugHelp.Win32 {
	public class NativeToken : IDisposable {
		private SafeTokenHandle tokenHandle;

		internal NativeToken(SafeTokenHandle tokenHandle) {
			this.tokenHandle = tokenHandle;
		}

		public void Dispose() {
			tokenHandle.Dispose();
		}
	}
}