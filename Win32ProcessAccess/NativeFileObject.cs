using Henke37.DebugHelp.Win32.AccessRights;
using Henke37.DebugHelp.Win32.SafeHandles;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
	public class NativeFileObject : IDisposable {
		internal SafeFileObjectHandle handle;

		internal NativeFileObject(SafeFileObjectHandle handle) {
			this.handle = handle;
		}

		public NativeFileObject(FileStream stream) {
			this.handle = new SafeFileObjectHandle(stream.SafeFileHandle);
		}

		public void Dispose() => handle.Dispose();
		public void Close() => handle.Close();

		public FileObjectType FileType {
			get {
				throw new NotImplementedException();
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

		internal unsafe void DeviceControlInput<TIn>(DeviceIoControlCode controlCode, ref TIn inBuff) where TIn : unmanaged {
			fixed (void* inBuffP = &inBuff) {
				bool success = DeviceIoControl(handle, controlCode,inBuffP,(uint)Marshal.SizeOf<TIn>(),null,0,out _ ,null);
				if(!success) throw new Win32Exception();
			}
		}

		internal unsafe TOut DeviceControlOutput<TOut>(DeviceIoControlCode controlCode) where TOut : unmanaged {
			TOut outBuff=new TOut();
			DeviceControlOutput(controlCode, ref outBuff);
			return outBuff;
		}
		internal unsafe void DeviceControlOutput<TOut>(DeviceIoControlCode controlCode, ref TOut outBuff) where TOut : unmanaged {
			fixed (void* outBuffP = &outBuff) {
				bool success = DeviceIoControl(handle, controlCode, null, 0, outBuffP, (uint)Marshal.SizeOf<TOut>(), out _, null);
				if(!success) throw new Win32Exception();
			}
		}

		internal unsafe void DeviceControlInputOutput<TIn, TOut>(DeviceIoControlCode controlCode, ref TIn inBuff, ref TOut outBuff) where TIn : unmanaged where TOut : unmanaged {
			fixed (void* inBuffP = &inBuff, outBuffP = &outBuff) {
				bool success = DeviceIoControl(handle, controlCode, inBuffP, (uint)Marshal.SizeOf<TIn>(), outBuffP, (uint)Marshal.SizeOf<TOut>(), out _, null);
				if(!success) throw new Win32Exception();
			}
		}
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
	}
}
