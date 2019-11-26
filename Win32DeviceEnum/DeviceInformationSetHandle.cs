using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.DeviceEnum {
	internal class DeviceInformationSetHandle : SafeHandleZeroOrMinusOneIsInvalid {

		public DeviceInformationSetHandle() : base(true) {
		}

		protected override bool ReleaseHandle() {
			bool success=SetupDiDestroyDeviceInfoList(handle);
			return success;
		}


		[DllImport("Setupapi.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool SetupDiDestroyDeviceInfoList(IntPtr handle);
	}
}
