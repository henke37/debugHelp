using PInvoke;
using System;
using System.Drawing;

namespace Henke37.Win32.Windows {
	public class WindowInfo {

		public Rectangle WindowBounds;
		public Rectangle ClientArea;
		public WindowStyle WindowStyle;
		public WindowExStyle WindowExStyle;
		public WindowStatus WindowStatus;

		public uint HorizontalBorder;
		public uint VerticalBorder;

		public IntPtr WindowClassAtom;

		internal WindowInfo(Native info) {
			WindowBounds = info.rcWindow.ToRectangle();
			ClientArea = info.rcClient.ToRectangle();
			WindowStyle = (WindowStyle)info.dwStyle;
			WindowExStyle = (WindowExStyle)info.dwExStyle;
			WindowStatus = (WindowStatus)info.dwWindowStatus;
			HorizontalBorder = info.cxWindowBorders;
			VerticalBorder = info.cyWindowBorders;
			WindowClassAtom = info.atomWindowType;
		}


#pragma warning disable CS0649
		internal unsafe struct Native {
			internal UInt32 cbSize;
			internal RECT rcWindow;
			internal RECT rcClient;
			internal UInt32 dwStyle;
			internal UInt32 dwExStyle;
			internal UInt32 dwWindowStatus;
			internal UInt32 cxWindowBorders;
			internal UInt32 cyWindowBorders;
			internal IntPtr atomWindowType;
			internal UInt16 wCreatorVersion;
		}
	}
}