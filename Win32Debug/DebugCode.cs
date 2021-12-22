using System;

namespace Henke37.Win32.Debug {
	internal enum DebugCode : UInt32 {
		Exception = 1,
		CreateThread = 2,
		CreateProcess = 3,
		ExitThread = 4,
		ExitProcess = 5,
		LoadDll = 6,
		UnloadDll = 7,
		OutputDebugString = 8,
		RIP = 9
	}
}