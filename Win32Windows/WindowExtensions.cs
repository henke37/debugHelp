using Henke37.Win32.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.Windows {
	public static class WindowExtensions {
		public static List<NativeWindow> GetWindows(this NativeThread thread) {
			return NativeWindow.GetThreadWindows(thread.ThreadId);
		}
	}
}
