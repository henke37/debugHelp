using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace Henke37.DebugHelp {
	public abstract class ProcessMemoryAccessor : ProcessMemoryReader {

		[SecuritySafeCritical]
		public abstract void WriteBytes(byte[] srcBuff, IntPtr dstAddr, uint size);

		[SecurityCritical]
		public virtual unsafe void WriteBytes(void* srcBuff, IntPtr dstAddr, uint size) {
			Byte[] buffArr = GetScratchBuff(size);
			Marshal.Copy((IntPtr)srcBuff, buffArr, 0, (int)size);
			WriteBytes(buffArr, dstAddr, size);
		}

		public void WriteByte(byte value, IntPtr dstAddr) {
			scratchBuff[0] = value;
			WriteBytes(scratchBuff, dstAddr, 1);
		}

		[SecuritySafeCritical]
		public unsafe void WriteStruct<T>(IntPtr dstAddr, ref T buff) where T : unmanaged {
			fixed (void* buffP = &buff) {
				WriteBytes(buffP, dstAddr, (uint)sizeof(T));
			}
		}

		public void WriteInt16(IntPtr dstAddr, short value) {
			scratchBuff[0] = (byte)(value&0x00FF);
			scratchBuff[1] = (byte)((value>>8)&0x00FF);
			WriteBytes(scratchBuff, dstAddr, 2);
		}
	}
}
