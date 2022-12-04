using Henke37.Win32.AccessRights;
using Henke37.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Henke37.Win32.Files {
	public class NativeFileObject : IDisposable {
		internal SafeFileObjectHandle handle;
		private const int NoError=0;

		internal NativeFileObject(SafeFileObjectHandle handle) {
			this.handle = handle;
		}

		public NativeFileObject(FileStream stream) {
			this.handle = new SafeFileObjectHandle(stream.SafeFileHandle);
		}

		public void Dispose() => handle.Dispose();
		public void Close() => handle.Close();

		public string GetFinalPath(GetFinalPathFlags flags) {
			StringBuilder sb = new StringBuilder();
			UInt32 neededBuff=16;
			do {
				sb.Capacity = (int)neededBuff+1;
				neededBuff = GetFinalPathNameByHandleW(handle, sb, (uint)sb.Capacity, (UInt32)flags);
				if(neededBuff == 0) throw new Win32Exception();
			} while(neededBuff > sb.Capacity);

			return sb.ToString();
		}

		public FileObjectType FileType {
			get {
				var t = GetFileType(handle);
				if(t==FileObjectType.Unknown) {
					var err = Marshal.GetLastWin32Error();
					if(err != NoError) throw new Win32Exception(err);
				}
				return t;
			}
		}

		public FileStream AsFileStream() {
			var newHandle=handle.AsSafeFileHandle();
			throw new NotImplementedException();
			//return new FileStream(newHandle);
		}

		public static unsafe NativeFileObject Open(string fileName, FileObjectAccessRights accessRights, FileShareMode shareMode, FileDisposition disposition,FileAttributes attributes) {
			SafeFileObjectHandle handle=CreateFileW(fileName, (uint)accessRights,(uint)shareMode,null,(uint)disposition,(uint)attributes,SafeFileObjectHandle.InvalidHandle);
			if(handle.IsInvalid) throw new Win32Exception();
			return new NativeFileObject(handle);
		}

		public void LockFile(UInt64 offset, UInt64 size) {
			bool success = LockFileNative(handle, (uint)(offset & 0x0FFFFFFFF), (uint)(offset >> 32), (uint)(size & 0x0FFFFFFFF), (uint)(size >> 32));
			if(!success) throw new Win32Exception();
		}
		public void UnlockFile(UInt64 offset, UInt64 size) {
			bool success = UnlockFileNative(handle, (uint)(offset & 0x0FFFFFFFF), (uint)(offset >> 32), (uint)(size & 0x0FFFFFFFF), (uint)(size >> 32));
			if(!success) throw new Win32Exception();
		}

		[SecurityCritical]
		internal unsafe void DeviceControl(DeviceIoControlCode controlCode) {
			bool success = DeviceIoControl(handle, controlCode, null, 0, null, 0, out _, null);
			if(!success) throw new Win32Exception();
		}

		[SecurityCritical]
		internal unsafe void DeviceControlInput<TIn>(DeviceIoControlCode controlCode, ref TIn inBuff) where TIn : unmanaged {
			fixed (void* inBuffP = &inBuff) {
				bool success = DeviceIoControl(handle, controlCode,inBuffP,(uint)Marshal.SizeOf<TIn>(),null,0,out _ ,null);
				if(!success) throw new Win32Exception();
			}
		}

		[SecurityCritical]
		internal unsafe TOut DeviceControlOutput<TOut>(DeviceIoControlCode controlCode) where TOut : unmanaged {
			TOut outBuff=new TOut();
			DeviceControlOutput(controlCode, ref outBuff);
			return outBuff;
		}
		[SecurityCritical]
		internal unsafe void DeviceControlOutput<TOut>(DeviceIoControlCode controlCode, ref TOut outBuff) where TOut : unmanaged {
			fixed (void* outBuffP = &outBuff) {
				bool success = DeviceIoControl(handle, controlCode, null, 0, outBuffP, (uint)Marshal.SizeOf<TOut>(), out _, null);
				if(!success) throw new Win32Exception();
			}
		}
		[SecurityCritical]
		internal unsafe uint DeviceControlOutput(DeviceIoControlCode controlCode, byte[] outBuff) {
			uint written = 0;
			fixed (void* outBuffP = outBuff) {
				bool success = DeviceIoControl(handle, controlCode, null, 0, outBuffP, (uint)outBuff.Length, out written, null);
				if(!success) throw new Win32Exception();
			}
			return written;
		}

		[SecurityCritical]
		internal unsafe uint DeviceControlInputOutput<TIn>(DeviceIoControlCode controlCode, ref TIn inBuff, byte[] outBuff) where TIn : unmanaged  {
			uint written = 0;
			fixed(void* inBuffP = &inBuff) {
				fixed(byte* bufferP = outBuff) {
					bool success = DeviceIoControl(handle, controlCode, inBuffP, (uint)Marshal.SizeOf<TIn>(), bufferP, (uint)outBuff.Length, out written, null);
					if(!success) throw new Win32Exception();
				}
			}

			return written;
		}
		[SecurityCritical]
		internal unsafe void DeviceControlInputOutput<TIn, TOut>(DeviceIoControlCode controlCode, ref TIn inBuff, ref TOut outBuff) where TIn : unmanaged where TOut : unmanaged {
			fixed (void* inBuffP = &inBuff, outBuffP = &outBuff) {
				bool success = DeviceIoControl(handle, controlCode, inBuffP, (uint)Marshal.SizeOf<TIn>(), outBuffP, (uint)Marshal.SizeOf<TOut>(), out _, null);
				if(!success) throw new Win32Exception();
			}
		}
		[SecurityCritical]
		internal unsafe TOut DeviceControlInputOutput<TIn, TOut>(DeviceIoControlCode controlCode, ref TIn inBuff) where TIn : unmanaged where TOut : unmanaged {
			TOut outBuff = new TOut();
			DeviceControlInputOutput(controlCode, ref inBuff, ref outBuff);
			return outBuff;
		}

		[DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool DeviceIoControl(SafeFileObjectHandle handle, DeviceIoControlCode controlCode, void* inBuffer, uint inBufferLength, void* outBuffer, uint outBufferLength, out uint returnSize, void *lpOverlapped);

		[DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern unsafe SafeFileObjectHandle CreateFileW([MarshalAs(UnmanagedType.LPWStr)] string fileName, UInt32 desiredAccess, UInt32 shareMode, SecurityAttributes *securityAttributes, UInt32 creationDisposition, UInt32 flagsAndAttributes, SafeFileObjectHandle template);

		[DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern FileObjectType GetFileType(SafeFileObjectHandle handle);

		[DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "LockFile")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool LockFileNative(SafeFileObjectHandle handle, UInt32 offsetLow, UInt32 offsetHigh, UInt32 sizeLow, UInt32 sizeHigh);

		[DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "UnlockFile")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool UnlockFileNative(SafeFileObjectHandle handle, UInt32 offsetLow, UInt32 offsetHigh, UInt32 sizeLow, UInt32 sizeHigh);


		[DllImport("Kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern UInt32 GetFinalPathNameByHandleW(SafeFileObjectHandle handle, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder buff, UInt32 buffLen, UInt32 flags);

		[Flags]
		public enum GetFinalPathFlags : UInt32 {
			Normalized = 0,
			Opened = 8,
			VolumeNameDos = 0,
			VolumeNameGUID = 1,
			VolumeNameNT = 2,
			VolumeNameNone = 4
		}
	}
}
