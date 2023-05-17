using Henke37.Win32.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Henke37.Win32.Windows {
	[SuppressUnmanagedCodeSecurity]
	[DebuggerDisplay("{Text} {ClassName}")]
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

		public bool IsMinimized {
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				return IsIconic(handle);
			}
		}

		public bool IsUnicode {
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				return IsWindowUnicode(handle);
			}
		}

		public bool IsHung {
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				return IsHungAppWindow(handle);
			}
		}

		public DwmCloakReason CloakReason {
			[SecuritySafeCritical]
			get {
				return GetDwnAttribute<DwmCloakReason>(DwmAttribute.Cloaked);
			}
		}

		public IntPtr Handle { get => handle; set => handle = value; }

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Move(int x, int y, int width, int height) {
			var success = MoveWindowNative(handle, x, y, width, height, true);
			if(!success) throw new Win32Exception();
		}

		public void Minimize() {
			var success = CloseWindowNative(handle);
			if(!success) throw new Win32Exception();
		}

		public void Restore() {
			var success = OpenIconNative(handle);
			if(!success) throw new Win32Exception();
		}

		public NativeWindow GetParent() {
			return new NativeWindow(GetAncestor(Handle, 1));
		}
		public NativeWindow GetRoot() {
			return new NativeWindow(GetAncestor(Handle, 2));
		}
		public NativeWindow GetRootOwner() {
			return new NativeWindow(GetAncestor(Handle, 3));
		}

		public Dictionary<string, IntPtr> GetWindowProps() {
			var props = new Dictionary<string, IntPtr>();

			GCHandle gch = GCHandle.Alloc(props, GCHandleType.Normal);

			EnumPropsExW(Handle, windPropEnumCallback, GCHandle.ToIntPtr(gch));

			gch.Free();

			return props;
		}

		public IntPtr GetProp(string name) {
			return GetPropW(Handle, name);
		}
		public IntPtr GetProp(IntPtr atom) {
			return GetPropWAtom(Handle, atom);
		}
		public void SetProp(string name, IntPtr handle) {
			var success = SetPropW(Handle, name, handle);
			if(!success) throw new Win32Exception();
		}
		public void SetProp(IntPtr atom, IntPtr handle) {
			var success = SetPropWAtom(Handle, atom, handle);
			if(!success) throw new Win32Exception();
		}
		public IntPtr RemoveProp(string name) {
			return RemovePropW(Handle, name);
		}
		public IntPtr RemoveProp(IntPtr atom) {
			return RemovePropWAtom(Handle, atom);
		}

		[Undocumented]
		public WindowBand WindowBand {
			get {
				var success = GetWindowBand(Handle, out uint pdwBand);
				if(!success) throw new Win32Exception();
				return (WindowBand)pdwBand;
			}
		}

		public DisplayAffinity DisplayAffinity {
			get {
				var success = GetWindowDisplayAffinity(Handle, out uint affinity);
				if(!success) throw new Win32Exception();
				return (DisplayAffinity)affinity;
			}
		}

		private static bool windPropEnumCallback(IntPtr hwnd, string name, IntPtr handle, IntPtr lParam) {
			GCHandle gch = GCHandle.FromIntPtr(lParam);
			var list = (Dictionary<string, IntPtr>)gch.Target;

			list.Add(name, handle);

			return true;
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
			fixed(char* buffp = buff) {
				var len = GetWindowTextW(handle, buffp, buffSize);

				return new string(buffp, 0, (int)len);
			}
		}


		[SecurityCritical]
		internal unsafe T GetDwnAttribute<T>(DwmAttribute attribute) where T : unmanaged {
			T buff;
			var success = DwmGetWindowAttributeNative(handle, (UInt32)attribute, &buff, (uint)sizeof(T));
			if(success.Failed) throw success.GetException();

			return buff;
		}

		public unsafe WindowInfo GetWindowInfo() {
			WindowInfo.Native info=new WindowInfo.Native() { cbSize = (uint)sizeof(WindowInfo.Native) };
			var success = GetWindowInfo(handle, ref info);
			if(!success) throw new Win32Exception();

			return new WindowInfo(info);
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

		public static NativeWindow? WindowFromPoint(System.Drawing.Point p1) {
			PInvoke.POINT p2;
			p2.x = p1.X;
			p2.y = p1.Y;
			IntPtr hwnd = WindowFromPointNative(p2);
			if(hwnd == IntPtr.Zero) return null;
			return new NativeWindow(hwnd);
		}


		[DllImport("User32.dll", SetLastError = true)]
		static extern UInt32 GetWindowTextLengthW(IntPtr hWnd);

		[DllImport("User32.dll", SetLastError = true)]
		static extern unsafe UInt32 GetWindowTextW(IntPtr hWnd, char* lpString, UInt32 buffLen);

		[DllImport("User32.dll", SetLastError = true)]
		static extern unsafe UInt32 GetClassNameW(IntPtr hWnd, char* lpString, UInt32 buffLen);

		[DllImport("User32.dll", SetLastError = true)]
		static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, out UInt32 ProcessId);

		[DllImport("User32.dll", SetLastError = true)]
		[return:MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowBand(IntPtr hWnd, out UInt32 pdwBand);

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowDisplayAffinity(IntPtr hWnd, out UInt32 pdwBand);

		[DllImport("User32.dll", SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("User32.dll", SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsZoomed(IntPtr hWnd);

		[DllImport("User32.dll", SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsIconic(IntPtr hWnd);

		[DllImport("User32.dll", SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsWindowUnicode(IntPtr hWnd);

		[DllImport("User32.dll", SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsHungAppWindow(IntPtr hWnd);


		[DllImport("User32.dll", SetLastError = false, EntryPoint = "MoveWindow")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool MoveWindowNative(IntPtr hWnd, Int32 x, Int32 y, Int32 width, Int32 height, [MarshalAs(UnmanagedType.Bool)] bool repaint);


		[DllImport("User32.dll", SetLastError = false, EntryPoint = "CloseWindow")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool CloseWindowNative(IntPtr hWnd);

		[DllImport("User32.dll", SetLastError = false, EntryPoint = "OpenIcon")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool OpenIconNative(IntPtr hWnd);

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


		[DllImport("user32.dll")]
		static extern IntPtr GetAncestor(IntPtr parent, UInt32 flags);


		private delegate bool EnumPropDelegate(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr handle, IntPtr lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool EnumPropsExW(IntPtr hwnd, EnumPropDelegate lpfn, IntPtr lParam);

		[DllImport("user32.dll")]
		static extern IntPtr GetPropW(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string name);
		[DllImport("user32.dll", EntryPoint = "GetPropW")]
		static extern IntPtr GetPropWAtom(IntPtr hwnd, IntPtr atom);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetPropW(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr handle);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", EntryPoint = "SetPropW", SetLastError = true)]
		static extern bool SetPropWAtom(IntPtr hwnd, IntPtr atom, IntPtr handle);

		[DllImport("user32.dll")]
		static extern IntPtr RemovePropW(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string name);
		[DllImport("user32.dll", EntryPoint = "RemovePropW")]
		static extern IntPtr RemovePropWAtom(IntPtr hwnd, IntPtr atom);

		[DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
		static extern IntPtr WindowFromPointNative(PInvoke.POINT point);


		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowInfo(IntPtr hwnd, ref WindowInfo.Native windowInfo);


		[DllImport("Dwmapi.dll", SetLastError = false, EntryPoint = "DwmGetWindowAttribute")]
		static unsafe extern PInvoke.HResult DwmGetWindowAttributeNative(IntPtr hwnd, UInt32 attribute, void* buff, UInt32 buffSize);
	}
}
