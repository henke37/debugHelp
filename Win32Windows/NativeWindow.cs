using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Henke37.Win32.Windows {
	public class NativeWindow : IWin32Window {
		private IntPtr handle;

		internal NativeWindow(IntPtr handle) {
			this.handle = handle;
		}

		public uint ThreadId {
			get {
				return GetWindowThreadProcessId(handle, out _);
			}
		}

		public uint ProcessId {
			get {
				GetWindowThreadProcessId(handle, out uint pid);
				return pid;
			}
		}

		public string ClassName {
			get => GetClassName();
		}

		public string Text {
			get => GetText();
		}


		public IntPtr Handle { get => handle; set => handle = value; }
		private unsafe string GetClassName() {
			uint buffSize = 256;
			var buff = new char[buffSize];
			fixed(char* buffp = buff) {
				var len = GetClassNameW(handle, buffp, buffSize);

				return new string(buffp, 0, (int)len);
			}
		}

		private unsafe string GetText() {
			var buffSize = GetWindowTextLengthW(handle);
			if(buffSize == 0 && Marshal.GetLastWin32Error() != 0) {
				throw new Win32Exception();
			}
			var buff = new char[buffSize];
			fixed(char* buffp=buff) {
				var len = GetWindowTextW(handle, buffp, buffSize);

				return new string(buffp,0,(int)len);
			}
		}

		public static List<NativeWindow> GetTopWindows() {
			var list = new List<NativeWindow>();

			GCHandle gch = GCHandle.Alloc(list, GCHandleType.Normal);

			EnumWindows(windEnumCallback, GCHandle.ToIntPtr(gch));

			gch.Free();

			return list;
		}

		internal static bool windEnumCallback(IntPtr hwnd, IntPtr lParam) {
			GCHandle gch = GCHandle.FromIntPtr(lParam);
			var list = (List<NativeWindow>)gch.Target;

			list.Add(new NativeWindow(hwnd));

			return true;
		}

		public static List<NativeWindow> GetThreadWindows(UInt32 threadId) {
			var list = new List<NativeWindow>();

			GCHandle gch = GCHandle.Alloc(list, GCHandleType.Normal);

			EnumThreadWindows(threadId, windEnumCallback, GCHandle.ToIntPtr(gch));

			gch.Free();

			return list;
		}




		[DllImport("User32.dll", SetLastError = true)]
		static extern UInt32 GetWindowTextLengthW(IntPtr hWnd);

		[DllImport("User32.dll", SetLastError = true)]
		static extern unsafe UInt32 GetWindowTextW(IntPtr hWnd, char* lpString, UInt32 buffLen);

		[DllImport("User32.dll", SetLastError = true)]
		static extern unsafe UInt32 GetClassNameW(IntPtr hWnd, char* lpString, UInt32 buffLen);

		[DllImport("User32.dll", SetLastError = true)]
		static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, out UInt32 ProcessId);



		private delegate bool EnumWindowDelegate(IntPtr hwnd, IntPtr lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool EnumWindows(EnumWindowDelegate lpfn, IntPtr lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowDelegate lpfn, IntPtr lParam);
	}
}
