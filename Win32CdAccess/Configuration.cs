using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.CdAccess {
	public class Configuration {

		public class FeatureDesc {
			[Description("If the feature is currently present")]
			public bool Current;
			[Description("If the feature is always present")]
			public bool Persistent;
			byte Version;

			internal FeatureDesc(FeatureHeader header) {
				Current = header.Current;
				Persistent = header.Persistent;
				Version = header.Version;
			}

			internal static unsafe FeatureDesc BuffToDesc(FeatureHeader header, byte* additionalData) {
				switch(header.Feature) {
					case FeatureNumber.ProfileList:
						return new ProfileListFeature(header, additionalData);
					case FeatureNumber.Core:
						return new CoreFeature(header, (CoreFeature.Native*)additionalData);
					case FeatureNumber.Morphing:
						return new MorphFeature(header, (MorphFeature.Native*)additionalData);
					case FeatureNumber.RemovableMedium:
						return new RemovableMediumFeature(header, (RemovableMediumFeature.Native*)additionalData);
					case FeatureNumber.WriteProtect:
						return new WriteProtectFeature(header, (WriteProtectFeature.Native*)additionalData);
					case FeatureNumber.RandomReadable:
						return new RandomReadableFeature(header, (RandomReadableFeature.Native*)additionalData);
					case FeatureNumber.CdRead:
						return new CdReadFeature(header, (CdReadFeature.Native*)additionalData);
					case FeatureNumber.RandomWritable:
						return new RandomWriteableFeature(header, (RandomWriteableFeature.Native*)additionalData);
					case FeatureNumber.IncrementalStreamingWritable:
						return new IncrementalStreamingWriteFeature(header, (IncrementalStreamingWriteFeature.NativeHeader*)additionalData);
					case FeatureNumber.WriteOnce:
						return new WriteOnceFeature(header, (WriteOnceFeature.Native*)additionalData);
					case FeatureNumber.CdTrackAtOnce:
						return new CdTrackAtOnceFeature(header, (CdTrackAtOnceFeature.Native*)additionalData);
					case FeatureNumber.CdMastering:
						return new CdMasteringFeature(header, (CdMasteringFeature.Native*)additionalData);
					case FeatureNumber.EmbeddedChanger:
						return new EmbededChangerFeature(header, (EmbededChangerFeature.Native*)additionalData);
					case FeatureNumber.CDAudioAnalogPlay:
						return new CdAudioAnalogPlayFeature(header, (CdAudioAnalogPlayFeature.Native*)additionalData);
					case FeatureNumber.LogicalUnitSerialNumber:
						return new DriveSerialNumberFeature(header, additionalData);
					case FeatureNumber.FirmwareDate:
						return new FirmwareDateFeature(header, (FirmwareDateFeature.Native*)additionalData);
					default:
						return new FeatureDesc(header);
				}
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct GetConfigIOInput {
			FeatureNumber Feature;
			RequestType RequestType;
			UInt32 Reserved1;
			UInt32 Reserved2;

			internal GetConfigIOInput(FeatureNumber feature, RequestType requestType) {
				Feature = feature;
				RequestType = requestType;
				Reserved1 = 0;
				Reserved2 = 0;
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct ConfigurationHeader {
			byte Length4;
			byte Length3;
			byte Lenght2;
			byte Lenght1;

			byte Reserved1;
			byte Reserved2;

			byte Profile2;
			byte Profile1;

			internal uint Length => (uint)((Length4 << 24) | (Length3 << 16) | (Lenght2 << 8) | Lenght1);
			internal ProfileType Profile => (ProfileType)((Profile2 << 8) | Profile1);
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct FeatureHeader {
			byte Feature2;
			byte Feature1;
			private byte Flags;
			internal byte AdditonalLength;

			internal bool Current => (Flags & 0x01) != 0;
			internal bool Persistent => (Flags & 0x02) != 0;
			internal byte Version => (byte)(Flags >> 2);

			internal FeatureNumber Feature => (FeatureNumber)((Feature2 << 8) | Feature1);
		}

		public enum RequestType : UInt32 {
			All,
			Current,
			One
		}

		public enum ProfileType {
			Invalid = 0x0000,
			NonRemovableDisk = 0x0001,
			RemovableDisk = 0x0002,
			MOErasable = 0x0003,
			MOWriteOnce = 0x0004,
			AS_MO = 0x0005,
			// Reserved                 0x0006 - 0x0007
			Cdrom = 0x0008,
			CdRecordable = 0x0009,
			CdRewritable = 0x000a,
			// Reserved                 0x000b - 0x000f
			DvdRom = 0x0010,
			DvdRecordable = 0x0011,
			DvdRam = 0x0012,
			DvdRewritable = 0x0013,  // restricted overwrite
			DvdRWSequential = 0x0014,
			DvdDashRDualLayer = 0x0015,
			DvdDashRLayerJump = 0x0016,
			// Reserved                 0x0017 - 0x0019
			DvdPlusRW = 0x001A,
			DvdPlusR = 0x001B,
			// Reserved                 0x001C - 001F
			DDCdrom = 0x0020,  // obsolete
			DDCdRecordable = 0x0021,  // obsolete
			DDCdRewritable = 0x0022,  // obsolete
									 // Reserved                 0x0023 - 0x0029
			DvdPlusRWDualLayer = 0x002A,
			DvdPlusRDualLayer = 0x002B,
			// Reserved                 0x002C - 0x003F
			BDRom = 0x0040,
			BDRSequentialWritable = 0x0041,  // BD-R 'SRM'
			BDRRandomWritable = 0x0042,  // BD-R 'RRM'
			BDRewritable = 0x0043,
			//  Reserved                0x0044 - 0x004F
			HDDVDRom = 0x0050,
			HDDVDRecordable = 0x0051,
			HDDVDRam = 0x0052,
			HDDVDRewritable = 0x0053,
			// Reserved                 0x0054 - 0x0057
			HDDVDRDualLayer = 0x0058,
			// Reserved                 0x0059 - 0x0059
			HDDVDRWDualLayer = 0x005A,
			// Reserved                 0x005B - 0xfffe
			NonStandard = 0xffff
		}

		public enum FeatureNumber {
			ProfileList = 0x0000,
			Core = 0x0001,
			Morphing = 0x0002,
			RemovableMedium = 0x0003,
			WriteProtect = 0x0004,
			// Reserved                  0x0005 - 0x000f
			RandomReadable = 0x0010,
			// Reserved                  0x0011 - 0x001c
			MultiRead = 0x001D,
			CdRead = 0x001E,
			DvdRead = 0x001F,
			RandomWritable = 0x0020,
			IncrementalStreamingWritable = 0x0021,
			SectorErasable = 0x0022,
			Formattable = 0x0023,
			DefectManagement = 0x0024,
			WriteOnce = 0x0025,
			RestrictedOverwrite = 0x0026,
			CdrwCAVWrite = 0x0027,
			Mrw = 0x0028,
			EnhancedDefectReporting = 0x0029,
			DvdPlusRW = 0x002A,
			DvdPlusR = 0x002B,
			RigidRestrictedOverwrite = 0x002C,
			CdTrackAtOnce = 0x002D,
			CdMastering = 0x002E,
			DvdRecordableWrite = 0x002F,   // both -R and -RW
			DDCDRead = 0x0030,   // obsolete
			DDCDRWrite = 0x0031,   // obsolete
			DDCDRWWrite = 0x0032,   // obsolete
			LayerJumpRecording = 0x0033,
			// Reserved                  0x0034 - 0x0036
			CDRWMediaWriteSupport = 0x0037,
			BDRPseudoOverwrite = 0x0038,
			// Reserved                       0x0039
			DvdPlusRWDualLayer = 0x003A,
			DvdPlusRDualLayer = 0x003B,
			// Reserved                  0x003c - 0x003f
			BDRead = 0x0040,
			BDWrite = 0x0041,
			TSR = 0x0042,
			// Reserved                  0x0043 - 0x004f
			HDDVDRead = 0x0050,
			HDDVDWrite = 0x0051,
			// Reserved                  0x0052 - 0x007f
			HybridDisc = 0x0080,
			// Reserved                  0x0081 - 0x00ff
			PowerManagement = 0x0100,
			SMART = 0x0101,
			EmbeddedChanger = 0x0102,
			CDAudioAnalogPlay = 0x0103,  // obsolete
			MicrocodeUpgrade = 0x0104,
			Timeout = 0x0105,
			DvdCSS = 0x0106,
			RealTimeStreaming = 0x0107,
			LogicalUnitSerialNumber = 0x0108,
			MediaSerialNumber = 0x0109,
			DiscControlBlocks = 0x010A,
			DvdCPRM = 0x010B,
			FirmwareDate = 0x010C,
			AACS = 0x010D,
			// Reserved                  0x010e - 0x010f
			VCPS = 0x0110,
		}

		internal unsafe static int FeatureSize(FeatureNumber feature) {
			switch(feature) {
				case FeatureNumber.ProfileList:
					return 0;
				case FeatureNumber.Core:
					return sizeof(CoreFeature.Native);
				case FeatureNumber.Morphing:
					return sizeof(MorphFeature.Native);
				case FeatureNumber.RemovableMedium:
					return sizeof(RemovableMediumFeature.Native);
				case FeatureNumber.WriteProtect:
					return sizeof(WriteProtectFeature.Native);
				case FeatureNumber.RandomReadable:
					return sizeof(RandomReadableFeature.Native);
				case FeatureNumber.MultiRead:
					return 0;
				case FeatureNumber.CdRead:
					return sizeof(CdReadFeature.Native);
				case FeatureNumber.RandomWritable:
					return sizeof(RandomWriteableFeature.Native);
				case FeatureNumber.WriteOnce:
					return sizeof(WriteOnceFeature.Native);
				case FeatureNumber.IncrementalStreamingWritable:
					return 0;
				case FeatureNumber.SectorErasable:
					return 0;
				case FeatureNumber.RestrictedOverwrite:
					return 0;
				case FeatureNumber.CdrwCAVWrite:
					return 4;
				case FeatureNumber.CdTrackAtOnce:
					return sizeof(CdTrackAtOnceFeature.Native);
				case FeatureNumber.CdMastering:
					return sizeof(CdMasteringFeature.Native);
				case FeatureNumber.TSR:
					return 0;
				case FeatureNumber.HybridDisc:
					return 0;
				case FeatureNumber.PowerManagement:
					return 0;
				case FeatureNumber.EmbeddedChanger:
					return sizeof(EmbededChangerFeature.Native);
				case FeatureNumber.CDAudioAnalogPlay:
					return sizeof(CdAudioAnalogPlayFeature.Native);
				case FeatureNumber.LogicalUnitSerialNumber:
					return 0;
				case FeatureNumber.MediaSerialNumber:
					return 0;
				case FeatureNumber.FirmwareDate:
					return sizeof(FirmwareDateFeature.Native);
				default:
					throw new NotImplementedException();
			}

		}

		public class CoreFeature : FeatureDesc {
			[Description("The interface used to communicate with the host")]
			public PhysicalInterface Interface;

			public bool DeviceBusyEvent;
			public bool Inquery2;

			internal unsafe CoreFeature(FeatureHeader header, Native* additionalData) : base(header) {
				Interface = (PhysicalInterface)additionalData->PhysInterface;
				DeviceBusyEvent = (additionalData->Flags & 0x01) != 0;
				Inquery2 = (additionalData->Flags & 0x02) != 0;
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal struct Native {
				byte PhysInterface4;
				byte PhysInterface3;
				byte PhysInterface2;
				byte PhysInterface1;

				internal byte Flags;

				byte Padding1;
				byte Padding2;
				byte Padding3;

				internal UInt32 PhysInterface => (UInt32)((PhysInterface4 << 24) | (PhysInterface3 << 16) | (PhysInterface2 << 8) | PhysInterface1);
			}

			public enum PhysicalInterface {
				Unspecified = 0,
				SCSI = 1,
				ATAPI = 2,
				Firewire = 3,
				FirewireA = 4,
				FibreChannel = 5,
				FirewireB = 6,
				SATAPI = 7,
				USB = 8
			}
		}

		public class ProfileListFeature : FeatureDesc {

			public List<ProfileListEntry> Profiles;

			internal unsafe ProfileListFeature(FeatureHeader header, byte* additionalData) : base(header) {
				Profiles = new List<ProfileListEntry>();

				for(var entryP=(ProfileListEntry.Native*)additionalData;(header.AdditonalLength+additionalData)!=entryP;++entryP) {
					Profiles.Add(entryP->AsManaged());
				}
			}

			public class ProfileListEntry {
				public ProfileType ProfileNumber;
				public bool Current;

				[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
				internal struct Native {
					byte ProfileNumber2;
					byte ProfileNumber1;
					byte Flags;
					byte Padding;

					public ProfileListEntry AsManaged() {
						return new ProfileListEntry() {
							ProfileNumber = (ProfileType)((ProfileNumber2 << 8) | ProfileNumber1),
							Current = (Flags & 0x01) != 0
						};
					}
				}

				public override string ToString() {
					return $"{ProfileNumber} {Current}";
				}
			}
			
		}

		public class MorphFeature : FeatureDesc {
			public bool Asynchronous;
			public bool OCEvent;

			internal unsafe MorphFeature(FeatureHeader header, Native* add) : base(header) {
				Asynchronous = (add->Flags & 0x01) != 0;
				OCEvent = (add->Flags & 0x02) != 0;
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal struct Native {
				internal byte Flags;

				byte Padding1;
				byte Padding2;
				byte Padding3;
			}
		}

		public class RemovableMediumFeature : FeatureDesc {
			[Description("The drive is capable of locking the medium into the drive")]
			public bool Lockable;
			public bool DBML;
			[Description("The drive defaults to locking the medium into the drive")]
			public bool DefaultToPrevent;
			public bool Eject;
			public bool Load;
			[Description("The physical mechanism used to load medium into the drive")]
			public LoadingMechanismType LoadingMechanism;

			internal unsafe RemovableMediumFeature(FeatureHeader header, Native* add) : base(header) {
				Lockable = (add->Flags & 0x01)!=0;
				DBML = (add->Flags & 0x02)!=0;
				DefaultToPrevent = (add->Flags & 0x04)!=0;
				Eject = (add->Flags & 0x08)!=0;
				Load = (add->Flags & 0x10)!=0;
				LoadingMechanism = (LoadingMechanismType)(add->Flags >> 5);
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal struct Native {
				internal byte Flags;
				byte Padding1;
				byte Padding2;
				byte Padding3;
			}

			public enum LoadingMechanismType : byte {
				CaddySlot = 0,
				Tray = 1,
				Popup = 2,
				ChangerDiscs = 4,
				ChangerMagazine = 5
			}
		}

		public class WriteProtectFeature : FeatureDesc {
			public bool SupportsSWPPBit;
			public bool SupportsPersistentWriteProtect;
			public bool WriteInhibitDCB;
			public bool DiscWriteProtectPAC;

			internal unsafe WriteProtectFeature(FeatureHeader header, Native*add) : base(header) {
				SupportsSWPPBit = (add->Flags & 0x01) != 0;
				SupportsPersistentWriteProtect = (add->Flags & 0x02) != 0;
				WriteInhibitDCB = (add->Flags & 0x04) != 0;
				DiscWriteProtectPAC = (add->Flags & 0x08) != 0;
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal struct Native {
				internal byte Flags;
				byte Padding1;
				byte Padding2;
				byte Padding3;
			}
		}

		public class RandomReadableFeature : FeatureDesc {
			public UInt32 LogicalBlockSize;
			public UInt16 Blocking;

			public bool PagePresent;

			internal unsafe RandomReadableFeature(FeatureHeader header, Native* add) : base(header) {
				LogicalBlockSize = (uint)(
					(add->LogicalBlockSize4 << 24) |
					(add->LogicalBlockSize3 << 16) |
					(add->LogicalBlockSize2 << 8) |
					(add->LogicalBlockSize1)
					);

				Blocking = (UInt16)(
					(add->Blocking2 << 8) |
					(add->Blocking1)
					);

				PagePresent = (add->Flags & 0x01) != 0;
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal struct Native {
				internal byte LogicalBlockSize4;
				internal byte LogicalBlockSize3;
				internal byte LogicalBlockSize2;
				internal byte LogicalBlockSize1;

				internal byte Blocking2;
				internal byte Blocking1;

				internal byte Flags;
				byte Padding;
			}
		}

		public class CdReadFeature : FeatureDesc {
			[Description("The drive supports reading CD-TEXT data")]
			public bool CDText;
			public bool C2ErrorData;
			public bool DigitalAudioPlay;

			internal unsafe CdReadFeature(FeatureHeader header, Native* add) : base(header) {
				CDText = (add->Flags & 0x01) != 0;
				C2ErrorData = (add->Flags & 0x02) != 0;
				DigitalAudioPlay = (add->Flags & 0x80) != 0;
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal struct Native {
				internal byte Flags;
				byte Padding1;
				byte Padding2;
				byte Padding3;
			}
		}

		public class RandomWriteableFeature : FeatureDesc {
			public UInt32 LastLogicalBlockAddress;
			public UInt32 LogicalBlockSize;
			public UInt16 Blocking;

			public bool PagePresent;

			internal unsafe RandomWriteableFeature(FeatureHeader header, Native* add) : base(header) {
				LastLogicalBlockAddress = (uint)(
					(add->LastLogicalBlockAddress4 << 24) |
					(add->LastLogicalBlockAddress3 << 16) |
					(add->LastLogicalBlockAddress2 << 8) |
					(add->LastLogicalBlockAddress1)
					);

				LogicalBlockSize = (uint)(
					(add->LogicalBlockSize4 << 24) |
					(add->LogicalBlockSize3 << 16) |
					(add->LogicalBlockSize2 << 8) |
					(add->LogicalBlockSize1)
					);

				Blocking = (UInt16)(
					(add->Blocking2 << 8) |
					(add->Blocking1)
					);

				PagePresent = (add->Flags & 0x01) != 0;
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal struct Native {
				internal byte LastLogicalBlockAddress4;
				internal byte LastLogicalBlockAddress3;
				internal byte LastLogicalBlockAddress2;
				internal byte LastLogicalBlockAddress1;

				internal byte LogicalBlockSize4;
				internal byte LogicalBlockSize3;
				internal byte LogicalBlockSize2;
				internal byte LogicalBlockSize1;

				internal byte Blocking2;
				internal byte Blocking1;

				internal byte Flags;
				byte Padding;
			}
		}

		public class IncrementalStreamingWriteFeature : FeatureDesc {

			public UInt16 DataBlockTypesSupported;
			public bool BufferUnderrunFree;
			public bool AddressModeReservation;
			public bool TrackRessourceInformation;

			public byte[] LinkSizes;

			internal unsafe IncrementalStreamingWriteFeature(FeatureHeader header, NativeHeader *add) : base(header) {
				DataBlockTypesSupported = (UInt16)(
					(add->DataBlockTypesSupported2 << 8) |
					(add->DataBlockTypesSupported1)
					);

				BufferUnderrunFree = (add->Flags & 0x01) != 0;
				AddressModeReservation = (add->Flags & 0x02) != 0;
				TrackRessourceInformation = (add->Flags & 0x04) != 0;

				byte* linksP = (byte*)(add + 1);
				LinkSizes = new byte[add->NumLinkSizes];
				for(byte linkIndex=0;linkIndex< add->NumLinkSizes; ++linkIndex) {
					LinkSizes[linkIndex] = linksP[linkIndex];
				}
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal struct NativeHeader {
				internal byte DataBlockTypesSupported2;
				internal byte DataBlockTypesSupported1;
				internal byte Flags;
				internal byte NumLinkSizes;
			}
		}

		public class WriteOnceFeature : FeatureDesc {
			public UInt32 LogicalBlockSize;
			public UInt16 Blocking;

			public bool PagePresent;

			internal unsafe WriteOnceFeature(FeatureHeader header, Native* add) : base(header) {
				LogicalBlockSize = (uint)(
					(add->LogicalBlockSize4 << 24) |
					(add->LogicalBlockSize3 << 16) |
					(add->LogicalBlockSize2 << 8) |
					(add->LogicalBlockSize4)
					);

				Blocking = (UInt16)(
					(add->Blocking2 << 8) |
					(add->Blocking1)
					);

				PagePresent = (add->Flags & 0x01) != 0;
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal struct Native {
				internal byte LogicalBlockSize4;
				internal byte LogicalBlockSize3;
				internal byte LogicalBlockSize2;
				internal byte LogicalBlockSize1;

				internal byte Blocking2;
				internal byte Blocking1;

				internal byte Flags;
				byte Padding;
			}
		}

		public class CdTrackAtOnceFeature : FeatureDesc {
			public bool RWSubchannelsRecordable;
			public bool CdRewritable;
			public bool TestWriteOk;
			public bool RWSubchannelPackedOk;
			public bool RWSubchannelRawOk;
			public bool BufferUnderrunFree;

			UInt16 DataTypesSupported;

			internal unsafe CdTrackAtOnceFeature(FeatureHeader header, Native* add) : base(header) {
				RWSubchannelsRecordable = (add->Flags & 0x01) != 0;
				CdRewritable = (add->Flags & 0x02) != 0;
				TestWriteOk = (add->Flags & 0x04) != 0;
				RWSubchannelPackedOk = (add->Flags & 0x08) != 0;
				RWSubchannelRawOk = (add->Flags & 0x10) != 0;
				BufferUnderrunFree = (add->Flags & 0x20) != 0;

				DataTypesSupported=(UInt16)((add->DataTypeSupported2 << 8) | add->DataTypeSupported1);
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal struct Native {
				internal byte Flags;
				byte Padding;
				internal byte DataTypeSupported2;
				internal byte DataTypeSupported1;
			}
		}

		public class CdMasteringFeature : FeatureDesc {
			public bool RWSubchannelsRecordable;
			public bool CdRewritable;
			public bool TestWriteOk;
			public bool RawRecordingOk;
			public bool RawMultiSessionOk;
			public bool SessionAtOnceOk;
			public bool BufferUnderrunFree;

			public uint MaximumCueSheetLength;

			internal unsafe CdMasteringFeature(FeatureHeader header, Native* add) : base(header) {
				RWSubchannelsRecordable = (add->Flags & 0x01) != 0;
				CdRewritable = (add->Flags & 0x02) != 0;
				TestWriteOk = (add->Flags & 0x04) != 0;
				RawRecordingOk = (add->Flags & 0x08) != 0;
				RawMultiSessionOk = (add->Flags & 0x10) != 0;
				SessionAtOnceOk = (add->Flags & 0x20) != 0;
				BufferUnderrunFree = (add->Flags & 0x40) != 0;

				MaximumCueSheetLength = (uint)((add->MaximumCueSheetLength3 << 16) | (add->MaximumCueSheetLength2 << 8) | add->MaximumCueSheetLength1);
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal struct Native {
				internal byte Flags;
				internal byte MaximumCueSheetLength3;
				internal byte MaximumCueSheetLength2;
				internal byte MaximumCueSheetLength1;
			}
		}

		public class DriveSerialNumberFeature : FeatureDesc {
			public string Serial;

			internal unsafe DriveSerialNumberFeature(FeatureHeader header, byte* add) : base(header) {
				Serial = new string((sbyte*)add, 0, header.AdditonalLength).TrimEnd(' ');
			}
		}

		public class EmbededChangerFeature : FeatureDesc {
			public bool SupportsDiscPresent;
			public bool SideChangeCapable;
			public uint HighestSlotNumber;

			internal unsafe EmbededChangerFeature(FeatureHeader header, Native* add) : base(header) {
				SupportsDiscPresent = (add->Flags & 0x04) != 0;
				SideChangeCapable = (add->Flags & 0x10) != 0;
				HighestSlotNumber = (uint)(add->HighSlot & 0b00011111);
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal struct Native {
				internal byte Flags;
				byte Padding1;
				byte Padding2;
				internal byte HighSlot;
			}
		}

		public class CdAudioAnalogPlayFeature : FeatureDesc {
			public bool SeparateVolumes;
			public bool SeparateChannelMute;
			public bool Scan;

			public UInt16 VolumeLevels;

			internal unsafe CdAudioAnalogPlayFeature(FeatureHeader header,Native *add) : base(header) {
				SeparateVolumes = (add->Flags & 0x01) != 0;
				SeparateChannelMute = (add->Flags & 0x02) != 0;
				Scan = (add->Flags & 0x04) != 0;

				VolumeLevels = (UInt16)(
					(add->VolumeLevels2 << 8) |
					(add->VolumeLevels1)
					);
			}


			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal struct Native {
				internal byte Flags;
				byte Padding;
				internal byte VolumeLevels2;
				internal byte VolumeLevels1;
			}
		}

		public class FirmwareDateFeature : FeatureDesc {
			[Description("The date the firmware was built on")]
			public DateTime CreationDate;

			internal unsafe FirmwareDateFeature(FeatureHeader header, Native* add) : base(header) {
				CreationDate = new DateTime(
					int.Parse(new string(add->Year, 0, 4)),
					int.Parse(new string(add->Month, 0, 2)),
					int.Parse(new string(add->Day, 0, 2)),
					int.Parse(new string(add->Hour, 0, 2)),
					int.Parse(new string(add->Minute, 0, 2)),
					int.Parse(new string(add->Second, 0, 2))
				);
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal unsafe struct Native {
				internal fixed sbyte Year[4];
				internal fixed sbyte Month[2];
				internal fixed sbyte Day[2];
				internal fixed sbyte Hour[2];
				internal fixed sbyte Minute[2];
				internal fixed sbyte Second[2];
				internal fixed byte Reserved[2];

			}
		}
	}

	
}
