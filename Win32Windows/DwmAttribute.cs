using System;

namespace Henke37.Win32.Windows {
	internal enum DwmAttribute : UInt32 {
		NCRenderingEnabled=1,
		NCRenderingPolicy,
		TransitionsForceDisabled,
		AllowNCPaint,
		CaptionButtonBounds,
		NonClientRTLLayout,
		ForceIconicRepresentation,
		Flip3DPolicy,
		ExtendedFrameBounds,
		HasIconicBitmap,
		DisallowPeek,
		ExcludedFromPeek,
		Cloak,
		Cloaked,
		FreezeRepresentation,
		UseHostBackdropBrush,
		UseImmersiveDarkMode=20,
		WindowCornerPreference=33,
		BorderColor,
		CaptionColor,
		TextColor,
		VisibleFrameBorderThickness,
		SystemBackdropType
	}

	[Flags]
	public enum DwmCloakReason : UInt32 {
		None = 0,
		App = 1,
		Shell = 2,
		Inherited = 4
	}
}