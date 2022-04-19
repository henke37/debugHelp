using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
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
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				return GetWindowThreadProcessId(handle, out _);
			}
		}

		public uint ProcessId {
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
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

		public bool IsVisible {
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				return IsWindowVisible(handle);
			}
		}

		public bool IsMaximized {
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				return IsZoomed(handle);
			}
		}

		public IntPtr Handle { get => handle; set => handle = value; }

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Move(int x, int y, int width, int height) {
			var success = MoveWindowNative(handle, x, y, width, height, true);
			if(!success) throw new Win32Exception();
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		private unsafe string GetClassName() {
			uint buffSize = 256;
			var buff = new char[buffSize];
			fixed(char* buffp = buff) {
				var len = GetClassNameW(handle, buffp, buffSize);

				return new string(buffp, 0, (int)len);
			}
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
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

		public static List<NativeWindow> FindTopWindows(string? className, string? windowName) {
			return FindWindows(IntPtr.Zero, className, windowName);
		}

		public List<NativeWindow> FindChildWindows(string? className, string? windowName) {
			return FindWindows(handle, className, windowName);
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		internal static List<NativeWindow> FindWindows(IntPtr parent, string? className, string? windowName) {
			var list = new List<NativeWindow>();

			IntPtr childWnd = IntPtr.Zero;
			for(; ; ) {
				childWnd = FindWindowExW(parent, childWnd, className, windowName);
				if(childWnd == IntPtr.Zero) break;
				list.Add(new NativeWindow(childWnd));
			}

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

		[DllImport("User32.dll", SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("User32.dll", SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsZoomed(IntPtr hWnd);


		[DllImport("User32.dll", SetLastError = false, EntryPoint = "MoveWindow")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool MoveWindowNative(IntPtr hWnd, Int32 x, Int32 y, Int32 width, Int32 height, [MarshalAs(UnmanagedType.Bool)] bool repaint);



		private delegate bool EnumWindowDelegate(IntPtr hwnd, IntPtr lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool EnumWindows(EnumWindowDelegate lpfn, IntPtr lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowDelegate lpfn, IntPtr lParam);


		[DllImport("user32.dll")]
		static extern IntPtr FindWindowExW(IntPtr parent, IntPtr childAfter, [MarshalAs (UnmanagedType.LPWStr)] string? className, [MarshalAs(UnmanagedType.LPWStr)] string? windowName);
		[DllImport("user32.dll", EntryPoint = "FindWindowExW")]
		static extern IntPtr FindWindowExWAtom(IntPtr parent, IntPtr childAfter, IntPtr atom, [MarshalAs(UnmanagedType.LPWStr)] string? windowName);
	}
}
