using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.CdAccess {
	public class InqueryData {
		public DeviceTypeEnum DeviceType;
		public DeviceQualifierEnum DeviceTypeQualifier;

		byte DeviceTypeModifier;
		public bool RemovableMedia;

		public byte ANSIVersion;
		public byte ECMAVersion;
		public byte ISOVersion;

		byte ResponseDataFormat;
		public bool HiSupport;
		public bool NormACA;
		public bool TerminateTask;
		public bool AERC;

		public bool PROTECT;
		public bool ThirdPartyCoppy;
		byte TPGS;
		public bool ACC;
		public bool SCCS;

		public bool Addr16;               // defined only for SIP devices.
		public bool Addr32;               // defined only for SIP devices.
		public bool AckReqQ;               // defined only for SIP devices.
		public bool MediumChanger;
		public bool MultiPort;
		public bool EnclosureServices;

		public bool SoftReset;
		public bool CommandQueue;
		public bool TransferDisable;
		public bool LinkedCommands;
		public bool Synchronous;
		public bool Wide16Bit;
		public bool Wide32Bit;
		public bool RelativeAddressing;

		public string VendorId;
		public string ProductId;
		public string ProductRevisionLevel;

		internal unsafe InqueryData(Native native) {
			DeviceType = (DeviceTypeEnum)(native.Flags1 & 0x1F);
			DeviceTypeQualifier = (DeviceQualifierEnum)(native.Flags1 >> 5);

			DeviceTypeModifier = (byte)(native.Flags2 & 0x7F);
			RemovableMedia = (native.Flags2 & 0x80) != 0;

			ANSIVersion = (byte)(native.Versions & 0x07);
			ECMAVersion = (byte)((native.Versions >> 3) & 0x07);
			ISOVersion = (byte)(native.Versions >> 6);

			ResponseDataFormat = (byte)(native.Flags4 & 0x0F);
			HiSupport = (native.Flags4 & 0x10) != 0;
			NormACA = (native.Flags4 & 0x20) != 0;
			TerminateTask = (native.Flags4 & 0x40) != 0;
			AERC = (native.Flags4 & 0x80) != 0;

			PROTECT = (native.Flags5 & 0x01) != 0;
			ThirdPartyCoppy = (native.Flags5 & 0x08) != 0;
			TPGS = (byte)((native.Flags5 >> 4) & 0x03);
			ACC = (native.Flags5 & 0x40) != 0;
			SCCS = (native.Flags5 & 0x80) != 0;

			Addr16 = (native.Flags6 & 0x01) != 0;
			Addr32 = (native.Flags6 & 0x02) != 0;
			AckReqQ = (native.Flags6 & 0x04) != 0;
			MediumChanger = (native.Flags6 & 0x08) != 0;
			MultiPort = (native.Flags6 & 0x10) != 0;
			//ReservedBit2 = (native.Flags6 & 0x20) != 0;
			EnclosureServices = (native.Flags6 & 0x40) != 0;
			//ReservedBit3 = (native.Flags6 & 0x80) != 0;

			SoftReset = (native.Flags7 & 0x01) != 0;
			CommandQueue = (native.Flags7 & 0x02) != 0;
			TransferDisable = (native.Flags7 & 0x04) != 0;
			LinkedCommands = (native.Flags7 & 0x08) != 0;
			Synchronous = (native.Flags7 & 0x10) != 0;
			Wide16Bit = (native.Flags7 & 0x20) != 0;
			Wide32Bit = (native.Flags7 & 0x40) != 0;
			RelativeAddressing = (native.Flags7 & 0x80) != 0;

			VendorId = Encoding.ASCII.GetString(native.VendorId, 8).TrimEnd(' ');
			ProductId = Encoding.ASCII.GetString(native.ProductId, 16).TrimEnd(' ');
			ProductRevisionLevel = Encoding.ASCII.GetString(native.ProductRevisionLevel, 4).TrimEnd(' ');
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal unsafe struct Native {
			internal byte Flags1;
			internal byte Flags2;
			internal byte Flags3;
			internal byte Versions;
			internal byte Flags4;
			internal byte AdditionalLength;
			internal byte Flags5;
			internal byte Flags6;
			internal byte Flags7;
			internal fixed byte VendorId[8];
			internal fixed byte ProductId[16];
			internal fixed byte ProductRevisionLevel[4];
			fixed byte VendorSpecific[20];
			fixed byte Reserved3[2];
			internal fixed UInt16 VersionDescriptors[8];
			fixed byte Reserved4[30];
		}

		public enum DeviceTypeEnum {
			DirectAccess = 0x00,
			SequentialAccess = 0x01,
			Printer = 0x02,
			Processor = 0x03,
			WriteOnceReadMultiple = 0x04,
			ReadOnlyDirectAccess = 0x05,
			Scanner = 0x06,
			Optical = 0x07,
			MediumChanger = 0x08,
			Comunication = 0x09,
			ArrayController = 0x0c,
			SCSIEnclosure = 0x0d,
			ReducedBlock = 0x0e,
			OpticalCardReaderWriter = 0x0f,
			BridgeController = 0x10,
			ObjectBasedStorage = 0x11,
			HostManagedZoneBlock = 0x14,
			UnknownOrNone = 0x1F,
			LogicalUnitNotPresent = 0x7F
		}

		public enum DeviceQualifierEnum {
			Active = 0,
			NotActive = 1,
			NotSupported = 3
		}
	}
}
