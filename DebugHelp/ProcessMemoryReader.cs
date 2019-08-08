using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Henke37.DebugHelp {
	public abstract class ProcessMemoryReader {

		public byte[] ReadBytes(IntPtr addr, uint size) {
			var buff = new Byte[size];
			ReadBytes(addr, size, buff);
			return buff;
		}

		[SecuritySafeCritical]
		public abstract void ReadBytes(IntPtr addr, uint size, byte[] buff);

		[SecurityCritical]
		public virtual unsafe void ReadBytes(IntPtr addr, uint size, void* buff) {
			Byte[] buffArr = GetScratchBuff(size);
			ReadBytes(addr, size, buffArr);
			Marshal.Copy(buffArr, 0, (IntPtr)buff, (int)size);
		}

		public Byte ReadByte(IntPtr addr) {
			ReadBytes(addr, 1, scratchBuff);
			return scratchBuff[0];
		}

		[SecuritySafeCritical]
		public unsafe void ReadStruct<T>(IntPtr addr, ref T buff) where T : unmanaged {
			fixed (void* buffP = &buff) {
				ReadBytes(addr, (uint)sizeof(T), buffP);
			}
		}

		public T[] ReadStructArr<T>(IntPtr addr, uint count) where T : unmanaged {
			var arr = new T[count];

			ReadStructArr<T>(addr, arr);

			return arr;
		}

		private unsafe void ReadStructArr<T>(IntPtr addr, T[] arr) where T : unmanaged {
			int arrLen = arr.Length;
			for(int i=0;i<arrLen;++i) {
				ReadStruct(addr, ref arr[i]);
				addr += sizeof(T);
			}
		}

		public string ReadNullTermString(IntPtr addr) {
			List<Byte> buff=new List<byte>();
			for(; ; addr+=1) {
				Byte b = ReadByte(addr);
				if(b == 0) break;
				buff.Add(b);
			}

			return Encoding.UTF8.GetString(buff.ToArray());
		}

#if x86
		public IntPtr ReadIntPtr(IntPtr addr) {
			return (IntPtr)ReadInt32(addr);
		}
#else
#error "No ReadIntPtr implementation"
#endif

		public int ReadInt32(IntPtr addr) {
			ReadBytes(addr, 4, scratchBuff);
			return scratchBuff[0] | (scratchBuff[1] << 8) | (scratchBuff[2] << 16) | (scratchBuff[3] << 24);
		}

		public uint ReadUInt32(IntPtr addr) {
			ReadBytes(addr, 4, scratchBuff);
			return (uint)(scratchBuff[0] | (scratchBuff[1] << 8) | (scratchBuff[2] << 16) | (scratchBuff[3] << 24));
		}

		public short ReadInt16(IntPtr addr) {
			ReadBytes(addr, 2, scratchBuff);
			return (short)(scratchBuff[0] | (scratchBuff[1] << 8));
		}

		public ushort ReadUInt16(IntPtr addr) {
			ReadBytes(addr, 2, scratchBuff);
			return (ushort)(scratchBuff[0] | (scratchBuff[1] << 8));
		}

		public short[] ReadInt16Array(IntPtr addr, uint count) {
			Int16[] arr = new short[count];
			ReadInt16Array(addr, count, arr);
			return arr;
		}

		public void ReadInt16Array(IntPtr addr, uint count, short[] arr) {
			uint byteC = count * 2;
			Byte[] buff = GetScratchBuff(byteC);

			ReadBytes(addr, byteC, buff);

			for(uint i = 0; i < count; ++i) {
				arr[i] = (short)(buff[0 + i * 2] | (buff[1 + i * 2] << 8));
			}
		}

		public ushort[] ReadUInt16Array(IntPtr addr, uint count) {
			UInt16[] arr = new ushort[count];
			ReadUint16Array(addr, count, arr);
			return arr;
		}

		public void ReadUint16Array(IntPtr addr, uint count, ushort[] arr) {
			uint byteC = count * 2;
			Byte[] buff = GetScratchBuff(byteC);

			ReadBytes(addr, byteC, buff);

			for(uint i = 0; i < count; ++i) {
				arr[i] = (ushort)(buff[0 + i * 2] | (buff[1 + i * 2] << 8));
			}
		}

		public int[] ReadInt32Array(IntPtr addr, uint count) {
			Int32[] arr = new int[count];
			ReadInt32Array(addr, count, arr);
			return arr;
		}

		public void ReadInt32Array(IntPtr addr, uint count, int[] arr) {
			uint byteC = count * 4;
			Byte[] buff = GetScratchBuff(byteC);

			ReadBytes(addr, byteC, buff);

			for(uint i = 0; i < count; ++i) {
				arr[i] = buff[0 + i * 4] | (buff[1 + i * 4] << 8) | (buff[2 + i * 4] << 16) | (buff[3 + i * 4] << 24);
			}
		}

		public uint[] ReadUInt32Array(IntPtr addr, uint count) {
			UInt32[] arr = new uint[count];
			ReadUInt32Array(addr, count, arr);
			return arr;
		}

		public void ReadUInt32Array(IntPtr addr, uint count, uint[] arr) {
			uint byteC = count * 4;
			Byte[] buff = GetScratchBuff(byteC);

			ReadBytes(addr, byteC, buff);

			for(uint i = 0; i < count; ++i) {
				arr[i] = (uint)(buff[0 + i * 4] | (buff[1 + i * 4] << 8) | (buff[2 + i * 4] << 16) | (buff[3 + i * 4] << 24));
			}
		}

		public IntPtr[] ReadIntPtrArray(IntPtr addr, uint count) {
			IntPtr[] arr = new IntPtr[count];
			ReadIntPtrArray(addr, count, arr);
			return arr;
		}

#if x86
		public void ReadIntPtrArray(IntPtr addr, uint count, IntPtr[] arr) {
			uint byteC = count * 4;
			Byte[] buff = GetScratchBuff(byteC);

			ReadBytes(addr, byteC, buff);

			for(uint i = 0; i < count; ++i) {
				arr[i] = (IntPtr)(buff[0 + i * 4] | (buff[1 + i * 4] << 8) | (buff[2 + i * 4] << 16) | (buff[3 + i * 4] << 24));
			}
		}
#else
#error "No ReadIntPtrArray implementation"
#endif

		public Stream GetReadStream(IntPtr addr, int count) {
			return new ProcessReadMemmoryStream(this, addr, count);
		}

		protected Byte[] scratchBuff = new Byte[16];

		protected byte[] GetScratchBuff(uint count) {
			return count <= scratchBuff.Length ? scratchBuff : new byte[count];
		}
	}
}
