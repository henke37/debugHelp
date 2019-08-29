using Henke37.DebugHelp.Win32.SafeHandles;
using System;

namespace Henke37.DebugHelp.Win32 {
	public class NativeFileObject : IDisposable {
		internal SafeFileObjectHandle handle;

		internal NativeFileObject(SafeFileObjectHandle handle) {
			this.handle = handle;
		}

		public void Dispose() => handle.Dispose();
		public void Close() => handle.Close();
	}
}
