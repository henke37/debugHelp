using System;

namespace Henke37.Win32.Windows {
	public struct WindowProp {
		public string Name;
		public IntPtr Handle;

		public WindowProp(string name, IntPtr handle) {
			Name = name;
			Handle = handle;
		}

		public override string ToString() {
			return $"{Name} - {Handle}";
		}
	}
}