using System;
using System.Collections.Generic;

namespace Henke37.DebugHelp {
	public class CachedProcessMemoryAccessor : ProcessMemoryAccessor {

		private Dictionary<IntPtr, byte[]> pageCache;
		private ProcessMemoryAccessor realAccessor;

		protected const int pageSize = 4096;

		public CachedProcessMemoryAccessor(ProcessMemoryAccessor realAccessor) {
			this.realAccessor = realAccessor ?? throw new ArgumentNullException(nameof(realAccessor));
			pageCache = new Dictionary<IntPtr, byte[]>();
		}

		public void ClearCache() {
			pageCache.Clear();
		}

		public void ClearCache(IntPtr startAddr, uint size) {
			var pageAddr = BasePageAddress(startAddr, out _);
			var endPageAddr = BasePageAddress(startAddr + (int)size + pageSize - 1, out _);
			for(; pageAddr != endPageAddr; pageAddr += pageSize) {
				pageCache.Remove(pageAddr);
			}
		}

		private IntPtr BasePageAddress(IntPtr address, out int offset) {
			offset = address.ToInt32() % pageSize;
			return (IntPtr)(address.ToInt64() & ~(pageSize-1));
		}

		public override void ReadBytes(IntPtr addr, uint size, byte[] buff) {
			var pageAddr = BasePageAddress(addr, out int offset);
			for(int copiedData = 0; copiedData < size;) {
				byte[] readPage;
				if(!pageCache.TryGetValue(pageAddr, out readPage)) {
					readPage=realAccessor.ReadBytes(pageAddr, pageSize);
					AddPageToCache(pageAddr, readPage);
				}

				int remainingBytes = (int)size - copiedData;
				int bytesLeftInPage = pageSize - offset;
				int copySize = remainingBytes > bytesLeftInPage ? bytesLeftInPage : remainingBytes;
				Array.Copy(readPage, offset, buff, copiedData, copySize);

				offset = 0;
				pageAddr += pageSize;
				copiedData += copySize;
			}
		}

		protected virtual void AddPageToCache(IntPtr pageAddr, byte[] pageData) {
			pageCache[pageAddr] = pageData;
		}

		public override void WriteBytes(byte[] srcBuff, IntPtr dstAddr, uint size) {
			ClearCache(dstAddr, size);

			realAccessor.WriteBytes(srcBuff, dstAddr, size);
		}
	}
}
