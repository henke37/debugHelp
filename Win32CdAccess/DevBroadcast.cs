using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Henke37.Win32.CdAccess {
	public class DevBroadcast {

		public const int DBT_DEVNODES_CHANGED = 0x0007;
		public const int DBT_QUERYCHANGECONFIG = 0x0017;
		public const int DBT_CONFIGCHANGED = 0x0018;
		public const int DBT_CONFIGCHANGECANCELED = 0x0019;
		public const int DBT_DEVICEARRIVAL = 0x8000;
		public const int DBT_DEVICEQUERYREMOVE = 0x8001;
		public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
		public const int DBT_DEVICEREMOVEPENDING = 0x8003;
		public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
		public const int DBT_DEVICETYPESPECIFIC = 0x8005;
		public const int DBT_CUSTOMEVENT = 0x8006;
		public const int DBT_USERDEFINED = 0xFFFF;

		private const int DBT_DEVTYP_OEM = 0x00000000;
		private const int DBT_DEVTYP_VOLUME = 0x00000002;
		private const int DBT_DEVTYP_PORT = 0x00000003;
		private const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;
		private const int DBT_DEVTYP_HANDLE = 0x00000006;

		public static unsafe DevBroadcast FromMessage(ref Message m) {
			var hdr = (NativeHdr *)m.LParam;
			switch(hdr->DeviceType) {
				case DBT_DEVTYP_VOLUME:
					return ((DevBroadcastVolume.NativeVol*)m.LParam)->AsManaged();

				default:
					throw new NotImplementedException();
			}
		}

		internal struct NativeHdr {
			internal UInt32 Size;
			internal UInt32 DeviceType;
			UInt32 Reserved;
		}

	}

	public class DevBroadcastVolume : DevBroadcast {

		public UInt32 UnitMask;
		public DeviceBroadcastFlag Flags;

		public List<char> DriveLetters {
			get {
				var o = new List<char>();

				for(int bit =0;bit < 27; ++bit) {
					if(((uint)(1 << bit) & UnitMask) !=0) {
						o.Add((char)('A' + bit));
					}
				}

				return o;
			}
		}

		internal DevBroadcastVolume(uint unitMask, ushort flags) {
			UnitMask = unitMask;
			Flags = (DeviceBroadcastFlag)flags;
		}

		internal struct NativeVol {
			NativeHdr Header;
			internal UInt32 UnitMask;
			internal UInt16 Flags;


			public DevBroadcastVolume AsManaged() {
				return new DevBroadcastVolume(UnitMask, Flags);
			}
		}

	}

	[Flags]
	public enum DeviceBroadcastFlag {
		Media = 1,
		Net = 2
	}
}
