using Henke37.DebugHelp.Win32.SafeHandles;
using Microsoft.Win32.SafeHandles;
using System;
using System.IO;

namespace Henke37.DebugHelp.Win32 {
	public class NativeFileObject : IDisposable {
		internal SafeFileObjectHandle handle;

		internal NativeFileObject(SafeFileObjectHandle handle) {
			this.handle = handle;
		}

		public void Dispose() => handle.Dispose();
		public void Close() => handle.Close();

		public FileStream AsFileStream() {
			var newHandle=handle.AsSafeFileHandle();
			throw new NotImplementedException();
			//return new FileStream(newHandle);
		}
	}
}
