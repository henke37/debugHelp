﻿using System;

namespace Henke37.Win32.Windows {
	[Flags]
	public enum WindowStyle : UInt32 {
		Border      = 0x00800000,
		Caption     = 0x00C00000,
		Child       = 0x40000000,
		ClipChildren= 0x02000000,
		ClipSiblings= 0x04000000,
		Disabled    = 0x08000000,
		DialogFrame = 0x00400000,
		Group       = 0x00020000,
		HScroll     = 0x00100000,
		Maximize    = 0x01000000,
		MaximizeBox = 0x00010000,
		Minimize    = 0x20000000,
		MinimizeBox = 0x00020000,
		Popup       = 0x80000000,
		SysMenu     = 0x00080000,
		TabStop     = 0x00010000,
		ThickFrame  = 0x00040000,
		Visible     = 0x10000000,
		VScroll     = 0x00200000,
		TiledWindow = Caption | SysMenu | ThickFrame | MinimizeBox | MaximizeBox,
		PopupWindow = Popup | Border | SysMenu
	}

	[Flags]
	public enum WindowExStyle : UInt32 {
		AcceptFiles         = 0x00000010,
		AppWindow           = 0x00040000,
		ClientEdge          = 0x00000200,
		Composited          = 0x02000000,
		ContextHelp         = 0x00000400,
		ControlParent       = 0x00010000,
		DialogModalFrame    = 0x00000001,
		Layered             = 0x00080000,
		LayoutRTL           = 0x00400000,
		LeftScrollbar       = 0x00004000,
		MDIChild            = 0x00000040,
		NoActivate          = 0x08000000,
		NoInheritLayout     = 0x00100000,
		NoParentNotify      = 0x00000004,
		NoRedirectionBitmap = 0x00200000,
		Right               = 0x00001000,
		RTLReading          = 0x00002000,
		StaticEdge          = 0x00020000,
		ToolWindow          = 0x00000080,
		TopMost             = 0x00000008,
		Transparent         = 0x00000020,
		WindowEdge          = 0x00000100,
		OverlappedWindow = WindowEdge | ClientEdge,
		PaletteWindow = WindowEdge | ToolWindow | TopMost
	}

	[Flags]
	public enum WindowStatus : UInt32 {
		Active = 1
	}

	[Flags]
	public enum ClassStyle : UInt32 {
		None = 0,
		ByteAlignClient = 0x1000,
		ByteAlignWindow = 0x2000,
		ClassDC         = 0x0040,
		DoubleClicks    = 0x0008,
		DropShadow  = 0x00020000,
		GlobalClass     = 0x4000,
		HRedraw         = 0x0002,
		NoClose         = 0x0200,
		OwnDC           = 0x0020,
		ParentDC        = 0x0080,
		SaveBits        = 0x0800,
		VRedraw         = 0x0001
	}

	internal enum GetClassLongNum : Int32 {
		Atom = -32,
		CBCLSExtra = -20,
		CBWNDExtra = -18,
		HBRBackground = -10,
		HCursor = -12,
		HIcon = -14,
		HIconSmall = -34,
		HModule = -16,
		MenuName = -8,
		Style = -26,
		WndProc = -24
	}

	public enum ShowWindowMode : UInt32 {
		Hide = 0,
		ShowNormal = 1,
		ShowMinimized = 2,
		Maximize = 3,
		ShowNoActivate = 4,
		Show = 5,
		Minimize = 6,
		ShowMinimizeNoActive = 7,
		ShowNoActivate2 = 8,
		Restore = 9,
		ShowDefault = 10,
		ForceMinimize = 11
	}
}