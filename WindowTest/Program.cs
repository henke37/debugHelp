using Henke37.Win32.Windows;
using System;
using System.Collections.Generic;

namespace WindowTest {
	class Program {
		static void Main(string[] args) {
			var windows = NativeWindow.GetTopWindows();

			windows.Sort(wndCmp);

			foreach(var window in windows) {
				Console.WriteLine($"{window.ProcessId} {window.ClassName}");
			}
		}

		private static int wndCmp(NativeWindow x, NativeWindow y) {
			uint xPid = x.ProcessId;
			uint yPid = y.ProcessId;
			if(xPid < yPid) return -1;
			if(xPid > yPid) return 1;

			uint xTid = x.ThreadId;
			uint yTid = y.ThreadId;

			if(xTid < yTid) return -1;
			if(xTid > yTid) return 1;

			return 0;
		}
	}
}
