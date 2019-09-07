using Henke37.DebugHelp.Win32.SafeHandles;

namespace Henke37.DebugHelp.Win32 {
	public class NativeToken {
		private SafeTokenHandle tokenHandle;

		internal NativeToken(SafeTokenHandle tokenHandle) {
			this.tokenHandle = tokenHandle;
		}
	}
}