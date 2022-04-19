using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.Windows {
	public class NativeWindow {
		private IntPtr Handle;

		private NativeWindow(IntPtr handle) {
			Handle = handle;
		}

		public uint ThreadId {
			get {
				return GetWindowThreadProcessId(Handle, out _);
			}
		}

		public uint ProcessId {
			get {
				GetWindowThreadProcessId(Handle, out uint pid);
				return pid;
			}
		}

		public string ClassName {
			get => GetClassName();
		}

		public string Text {
			get => GetText();
		}

		private unsafe string GetClassName() {
			uint buffSize = 256;
			var buff = new char[buffSize];
			fixed(char* buffp = buff) {
				var len = GetClassNameW(Handle, buffp, buffSize);

				return new string(buffp, 0, (int)len);
			}
		}

		private unsafe string GetText() {
			var buffSize = GetWindowTextLengthW(Handle);
			if(buffSize == 0 && Marshal.GetLastWin32Error() != 0) {
				throw new Win32Exception();
			}
			var buff = new char[buffSize];
			fixed(char* buffp=buff) {
				var len = GetWindowTextW(Handle, buffp, buffSize);

				return new string(buffp,0,(int)len);
			}
		}

		[DllImport("User32.dll", SetLastError = true)]
		static extern UInt32 GetWindowTextLengthW(IntPtr hWnd);

		[DllImport("User32.dll", SetLastError = true)]
		static extern unsafe UInt32 GetWindowTextW(IntPtr hWnd, char* lpString, UInt32 buffLen);

		[DllImport("User32.dll", SetLastError = true)]
		static extern unsafe UInt32 GetClassNameW(IntPtr hWnd, char* lpString, UInt32 buffLen);

		[DllImport("User32.dll", SetLastError = true)]
		static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, out UInt32 ProcessId);
	}
}
