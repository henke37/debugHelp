using Henke37.DebugHelp;
using Henke37.Win32.Processes;
using System;

namespace Henke37.Win32.Memory {
	public class ReadOnlyCachedProcessMemoryAccessor : CachedProcessMemoryAccessor {

		private NativeProcess process;

		public ReadOnlyCachedProcessMemoryAccessor(ProcessMemoryAccessor realAccessor, NativeProcess process) : base(realAccessor) {
			this.process = process;
		}

		protected override void AddPageToCache(IntPtr pageAddr, byte[] pageData) {
			var memInfos=process.QueryMemoryRangeInformation(pageAddr, pageSize);

			foreach(var memInfo in memInfos) {
				if(memInfo.Protect.IsWriteable()) return;
			}

			base.AddPageToCache(pageAddr, pageData);
		}
	}
}
