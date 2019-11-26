using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.DeviceEnum {
	public class DeviceInformationSet {

		private DeviceInformationSetHandle handle;

		private const int ERROR_NO_MORE_ITEMS = 259;
		private const int ERROR_INSUFFICIENT_BUFFER = 122;

		internal DeviceInformationSet(Guid guid, DeviceInformationClassFlags flags) {
			handle = SetupDiGetClassDevsGuid(guid, IntPtr.Zero, IntPtr.Zero, flags);
		}

		internal DeviceInformationSet(string enumerator, DeviceInformationClassFlags flags) {
			handle = SetupDiGetClassDevsEnumerator(IntPtr.Zero, enumerator, IntPtr.Zero, flags);
		}

		public IEnumerable<DeviceInterface> GetDevices() {

			for(uint interfaceIndex=0;interfaceIndex<100;++interfaceIndex) {
				DeviceInterface.Native native=new DeviceInterface.Native();
				native.cdSize = (uint)Marshal.SizeOf(typeof(DeviceInterface.Native));
				try {
					bool success = SetupDiEnumDeviceInterfaces(
						handle,
						IntPtr.Zero,
						IntPtr.Zero,
						interfaceIndex,
						ref native
					);
					if(!success) throw new Win32Exception();
				} catch(Win32Exception err) when(err.NativeErrorCode==ERROR_NO_MORE_ITEMS) {
					yield break;
				}

				{
					bool success;
					UInt32 requiredSize=0;
					try {
						success = SetupDiGetDeviceInterfaceDetailW(
							handle,
							native,
							IntPtr.Zero,
							0,
							ref requiredSize,
							IntPtr.Zero
						);
						if(!success) throw new Win32Exception();
					} catch(Win32Exception err) when(err.NativeErrorCode == ERROR_INSUFFICIENT_BUFFER) {
					}

					success = SetupDiGetDeviceInterfaceDetailW(
							handle,
							native,
							IntPtr.Zero,
							0,
							ref requiredSize,
							IntPtr.Zero
					);
					if(!success) throw new Win32Exception();
				}
			}

			yield break;
		}

		[DllImport("Setupapi.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "SetupDiGetClassDevsW")]
		internal static extern unsafe DeviceInformationSetHandle SetupDiGetClassDevsGuid(Guid ClassGuid,
			IntPtr Enumerator,
			IntPtr hwndParent,
			DeviceInformationClassFlags flags
		);

		[DllImport("Setupapi.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "SetupDiGetClassDevsW")]
		internal static extern unsafe DeviceInformationSetHandle SetupDiGetClassDevsEnumerator(IntPtr ClassGuid,
			[MarshalAs(UnmanagedType.LPTStr)] string Enumerator,
			IntPtr hwndParent,
			DeviceInformationClassFlags flags
		);

		[DllImport("Setupapi.dll", ExactSpelling = true, SetLastError = true)]
		[return:MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool SetupDiEnumDeviceInterfaces(
			DeviceInformationSetHandle DeviceInfoSet,
			IntPtr DeviceInfoData,
			IntPtr InterfaceClassGuid,
			UInt32                     MemberIndex,
			ref DeviceInterface.Native DeviceInterfaceData
		);

		[DllImport("Setupapi.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool SetupDiGetDeviceInterfaceDetailW(
			DeviceInformationSetHandle DeviceInfoSet,
			DeviceInterface.Native DeviceInterfaceData,
			IntPtr DeviceInterfaceDetailData,
			UInt32 DeviceInterfaceDetailDataSize,
			ref UInt32 RequiredSize,
			IntPtr DeviceInfoData
		);
	}
}
