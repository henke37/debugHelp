using Henke37.DebugHelp.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
	public class MemoryNotification {
		SafeMemoryNotificationHandle handle;

		public MemoryNotification(MemoryNotificationType type) {
			handle= CreateMemoryResourceNotification((uint)type);
			if(handle.IsInvalid) throw new Win32Exception();
		}

		public bool Status {
			get {
				var success = QueryMemoryResourceNotification(handle, out bool status);
				if(!success) throw new Win32Exception();
				return status;
			}
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static unsafe extern SafeMemoryNotificationHandle CreateMemoryResourceNotification(uint type);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static unsafe extern bool QueryMemoryResourceNotification(SafeMemoryNotificationHandle handle, [MarshalAs(UnmanagedType.Bool)] out bool status);
	}

	public enum MemoryNotificationType : uint {
		LowMemory=0,
		HighMemory=1
	}
}
