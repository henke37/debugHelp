using Henke37.Win32.Threads;
using System.Collections.Generic;
using System.Drawing;

namespace Henke37.Win32.Windows {
	public static class WindowExtensions {
		public static List<NativeWindow> GetWindows(this NativeThread thread) {
			return NativeWindow.GetThreadWindows(thread.ThreadId);
		}

		public static List<NativeWindow> GetWindows(this Snapshots.ThreadEntry thread) {
			return NativeWindow.GetThreadWindows(thread.ThreadId);
		}
		public static List<NativeWindow> GetWindows(this Clone.QueryStructs.ThreadEntry thread) {
			return NativeWindow.GetThreadWindows(thread.ThreadId);
		}

		public static Rectangle ToRectangle(this PInvoke.RECT rect) {
			return new Rectangle(rect.left, rect.top, rect.right-rect.left, rect.bottom-rect.top);
		}
	}
}
