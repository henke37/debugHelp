using System;

namespace Henke37.Win32.Processes {
	[Flags]
	public enum StartupInfoFlags : UInt32 {
		None = 0,
		ForceOnFeedback = 0x00000040,
		ForceOffFeedback = 0x00000080,
		PreventPinning = 0x00002000,
		RunFullScreen = 0x00000020,
		TitleIsAppId = 0x00001000,
		TitleIsLinkName = 0x00000800,
		UntrustedSource = 0x00008000,
		UseCountChars = 0x00000008,
		UseFillAttribute = 0x00000010,
		UseHotKey = 0x00000200,
		UsePosition = 0x00000004,
		UseShowWindow = 0x00000001,
		UseSize = 0x00000002,
		UseSTDHandles = 0x00000100
	}
}
