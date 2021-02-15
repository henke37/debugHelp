using Henke37.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace Henke37.Win32.Processes {
	public class ProcThreadAttributeList : IDisposable {
		private bool disposedValue;

		internal unsafe Native* lpAttributeList;

		public ProcThreadAttributeList(uint count) {
			Init(count);
		}

		public bool IsDisposed { get => disposedValue; }

		[SuppressUnmanagedCodeSecurity]
		private unsafe void Init(uint count) {
			uint size =0;
			InitializeProcThreadAttributeList(lpAttributeList, count, 0, ref size);

			try {
				lpAttributeList = (Native*)Marshal.AllocHGlobal((int)size);

				bool success = InitializeProcThreadAttributeList(lpAttributeList, count, 0, ref size);
				if(!success) throw new Win32Exception();
			} catch(Exception err) {
				Marshal.FreeHGlobal((IntPtr)lpAttributeList);
				disposedValue = true;
				GC.SuppressFinalize(this);

				throw err;
			}
		}

		[SuppressUnmanagedCodeSecurity]
		public unsafe void AddAttribute<T>(ProcThreadAttribute att, T* val) where T : unmanaged {
			if(disposedValue) throw new ObjectDisposedException("ProcThreadAttributeList");

			bool success = UpdateProcThreadAttribute(lpAttributeList, 0, (UInt32)att, val, (uint)sizeof(T), null, null);
			if(!success) throw new Win32Exception();
		}

#pragma warning disable CS0169
		internal struct Native {
			UInt32 dwFlags;
			UInt32 Size;
			UInt32 Count;
			UInt32 Reserved;
			IntPtr Unknown;
		}
#pragma warning restore CS0169

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern unsafe bool InitializeProcThreadAttributeList(
			ProcThreadAttributeList.Native* lpAttributeList,
			UInt32 dwAttributeCount,
			UInt32 dwFlags,
			ref UInt32 lpSize
		);

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern unsafe bool UpdateProcThreadAttribute(
			ProcThreadAttributeList.Native* lpAttributeList,
			UInt32 dwFlags,
			UInt32 Attribute,
			void* lpValue,
			UInt32 cbSize,
			void* lpPreviousValue,
			void* lpReturnSize
		);

		[DllImport("Kernel32.dll", SetLastError = true)]
		static extern unsafe void DeleteProcThreadAttributeList(ProcThreadAttributeList.Native* lpAttributeList);

		[SuppressUnmanagedCodeSecurity]
		protected virtual unsafe void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					// TODO: dispose managed state (managed objects)
				}

				DeleteProcThreadAttributeList(lpAttributeList);
				Marshal.FreeHGlobal((IntPtr)lpAttributeList);

				disposedValue = true;
			}
		}

		~ProcThreadAttributeList() {
		    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		    Dispose(disposing: false);
		}

		public void Dispose() {
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}