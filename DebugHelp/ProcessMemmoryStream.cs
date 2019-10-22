using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Henke37.DebugHelp {
	public class ProcessReadMemmoryStream : Stream {
		private ProcessMemoryReader Reader;

		private IntPtr _startAddr;
		private int _length;
		private int _position;

		public ProcessReadMemmoryStream(ProcessMemoryReader reader, IntPtr startAddr, int length) {
			if(reader == null) throw new ArgumentNullException(nameof(reader));
			Reader = reader;
			_startAddr = startAddr;
			_length = length;
		}

		public override bool CanRead => true;
		public override bool CanSeek => true;
		public override bool CanWrite => false;
		public override long Length => _length;

		public override long Position { get => _position; set => _position=(int)value; }

		public override void Flush() {}

		public override int Read(byte[] buffer, int offset, int count) {
			if(buffer == null) throw new ArgumentNullException(nameof(buffer));
			if(offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
			if(count < 0) throw new ArgumentOutOfRangeException(nameof(count));
			if(offset + count > buffer.Length) throw new ArgumentException();
			if(_position < 0) throw new InvalidOperationException();

			long bytesLeft = _length - _position;
			if(count > bytesLeft) {
				count = (int)bytesLeft;
			}
			if(count<=0) {
				return 0;
			}
			try {
				Reader.ReadBytes((IntPtr)(_startAddr + _position), (uint)count, buffer);
			} catch (Win32Exception err) {
				throw new IOException(err.Message,err);
			} catch (IncompleteReadException err) {
				throw new IOException(err.Message, err);
			}
			_position += count;

			return count;
		}

		public override int ReadByte() {
			if(_position < 0) throw new InvalidOperationException();
			if(_position >= _length) return -1;
			return Reader.ReadByte(_startAddr + _position++);
		}

		public override long Seek(long offset, SeekOrigin origin) {
			switch(origin) {
				case SeekOrigin.Begin:
					return _position = (int)offset;
				case SeekOrigin.Current:
					return _position += (int)offset;
				case SeekOrigin.End:
					return _position = _length - (int)offset;
				default:
					throw new ArgumentOutOfRangeException(nameof(origin));
			}
		}

		public override void SetLength(long value) {
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotSupportedException();
		}
	}
}
