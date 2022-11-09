using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

using Henke37.Win32.AccessRights;
using Henke37.Win32.DeviceEnum;
using Henke37.Win32.Files;
using static System.Collections.Specialized.BitVector32;
using static Henke37.Win32.CdAccess.Configuration;

namespace Henke37.Win32.CdAccess {
	public class CdDrive {
		internal NativeFileObject file;
		private const int TOCTrackCount = 100;

		private readonly static Guid CdDriveGuid = new Guid("53F56308-B6BF-11D0-94F2-00A0C91EFB8B");

		private CdDrive(string path) {
			file = NativeFileObject.Open(path, FileObjectAccessRights.GenericRead | FileObjectAccessRights.GenericWrite, FileShareMode.Write | FileShareMode.Read, FileDisposition.OpenExisting, 0);
		}

		public CdDrive(DeviceInterface deviceInterface) : this(deviceInterface.FilePath) { }

		public static IEnumerable<DeviceInterface> GetCdDrives() {
			var devInfo = new DeviceInformationSet(CdDriveGuid, DeviceInformationClassFlags.Present | DeviceInformationClassFlags.DeviceInterface);
			return devInfo.GetDevices();
		}

		public void Eject() {
			file.DeviceControl(DeviceIoControlCode.DiskEjectMedia);
		}

		public void Load() {
			file.DeviceControl(DeviceIoControlCode.DiskLoadMedia);
		}

		public void VerifyMedia() {
			try {
				file.DeviceControl(DeviceIoControlCode.CdRomCheckVerify);
			} catch(Win32Exception ex) when (ex.NativeErrorCode == 1110) {

			}
		}

		[SecuritySafeCritical]
		public unsafe TOC GetTOC() {
			int buffSize = Marshal.SizeOf<TOC.TocHeader>() + Marshal.SizeOf<TrackEntry.Native>() * TOCTrackCount;
			byte[] buff = new byte[buffSize];

			var tracks = new List<TrackEntry>();

			TOC.TocHeader header;

			file.DeviceControlOutput(DeviceIoControlCode.CdRomReadTOC, buff);
			fixed (byte* buffPP = buff) {
				header = Marshal.PtrToStructure<TOC.TocHeader>((IntPtr)buffPP);

				byte* buffP = buffPP + Marshal.SizeOf<TOC.TocHeader>();
				for(var trackIndex = 0; trackIndex < header.LastTrack+1; ++trackIndex) {
					var entry = Marshal.PtrToStructure<TrackEntry.Native>((IntPtr)buffP);
					tracks.Add(entry.AsNative());

					buffP += Marshal.SizeOf<TrackEntry.Native>();
				}
			}

			return new TOC(
				header.FirstTrack,
				header.LastTrack,
				tracks
			);
		}

		[SecuritySafeCritical]
		public unsafe FullToc GetFullTOC(int session) {
			ReadTocEx readToc = new ReadTocEx(ReadTocFormat.FullToc, true, session);

			int buffSize = Marshal.SizeOf<FullToc.FullTocHeader>() + Marshal.SizeOf<TocFullDataBlock.Native>() * TOCTrackCount;
			byte[] buff = new byte[buffSize];

			var tracks = new List<TocFullDataBlock>();

			FullToc.FullTocHeader header;

			file.DeviceControlInputOutput(DeviceIoControlCode.CdRomReadTOCEx, ref readToc, buff);
			fixed(byte* buffPP = buff) {
				header = Marshal.PtrToStructure<FullToc.FullTocHeader>((IntPtr)buffPP);
				Debug.Assert(header.FirstCompleteSession == 1);

				int length = header.LengthLo | header.LengthHi << 8; 

				byte* buffP = buffPP + Marshal.SizeOf<FullToc.FullTocHeader>();
				byte* buffEndP = buffP + (length - 2);
				for(var trackIndex = 0; buffP < buffEndP; ++trackIndex) {
					var entry = Marshal.PtrToStructure<TocFullDataBlock.Native>((IntPtr)buffP);
					Debug.Assert(entry.Zero == 0);
					tracks.Add(entry.AsManaged());

					buffP += Marshal.SizeOf<TocFullDataBlock.Native>();
				}
			}

			return new FullToc(
				header.FirstCompleteSession,
				header.LastCompleteSession,
				tracks
			);
		}

		[SecuritySafeCritical]
		public unsafe FeatureDesc? GetConfiguration(FeatureNumber feature, RequestType requestType) {
			long buffSize = sizeof(ConfigurationHeader) + sizeof(FeatureHeader) + FeatureSize(feature);
			byte[] buff;

			for(; ;) {
				buff = new byte[buffSize];

				GetConfigIOInput getConf = new GetConfigIOInput(feature, requestType);

				var written = file.DeviceControlInputOutput(DeviceIoControlCode.CdRomGetConfiguration, ref getConf, buff);


				fixed(byte* buffPP = buff) {
					ConfigurationHeader header = Marshal.PtrToStructure<ConfigurationHeader>((IntPtr)buffPP);

					uint headersSize = (uint)(Marshal.SizeOf<ConfigurationHeader>());
					if(header.Length < headersSize) throw new Exception("Bad size!");

					byte* buffP = buffPP + Marshal.SizeOf<ConfigurationHeader>();

					headersSize += (uint)Marshal.SizeOf<FeatureHeader>();

					if(header.Length < headersSize) throw new Exception("Bad size!");

					FeatureHeader featureHeader = Marshal.PtrToStructure<FeatureHeader>((IntPtr)buffP);

					if(featureHeader.Feature != feature) return null;

					if(header.Length < headersSize + featureHeader.AdditonalLength) throw new Exception("Bad size!");
					if(buffSize < headersSize + featureHeader.AdditonalLength) {
						buffSize = headersSize + featureHeader.AdditonalLength;
						continue;
					}

					var addData = buffP + sizeof(FeatureHeader);

					return FeatureDesc.BuffToDesc(featureHeader, addData);
				}

			}
		}

		[SecuritySafeCritical]
		public unsafe List<ATIP>? GetATIP() {
			try {
				ReadTocEx readToc = new ReadTocEx(ReadTocFormat.ATIP, true, 0);

				List<ATIP> entries = new List<ATIP>();

				int BlockCount = 5;

				int buffSize = Marshal.SizeOf<ATIP.DataHeader>() + Marshal.SizeOf<ATIP.DataBlock>() * BlockCount;
				byte[] buff = new byte[buffSize];

				ATIP.DataHeader header;

				file.DeviceControlInputOutput(DeviceIoControlCode.CdRomReadTOCEx, ref readToc, buff);

				fixed(byte* buffPP = buff) {
					header = Marshal.PtrToStructure<ATIP.DataHeader>((IntPtr)buffPP);

					byte* buffP = buffPP + Marshal.SizeOf<ATIP.DataHeader>();
					byte* buffEndP = buffP + (header.Length - 2);
					for(var entryIndex = 0; buffP < buffEndP; ++entryIndex) {
						var entry = Marshal.PtrToStructure<ATIP.DataBlock>((IntPtr)buffP);
						entries.Add(entry.AsManaged());

						buffP += Marshal.SizeOf<ATIP.DataBlock>();
					}
				}

				return entries;
			} catch(Win32Exception err) when ((uint)err.ErrorCode == 0x80004005) {
				return null;
			}
		}

		[SecuritySafeCritical]
		internal unsafe List<CdTextDataBlock> GetCdTextBlocks(int session=1) {
			ReadTocEx readToc = new ReadTocEx(ReadTocFormat.CDText, true, session);
			byte[] buff = new byte[4200];

			FullToc.FullTocHeader header;

			file.DeviceControlInputOutput(DeviceIoControlCode.CdRomReadTOCEx, ref readToc, buff);

			List<CdTextDataBlock> blocks=new List<CdTextDataBlock>();

			fixed(byte* buffPP = buff) {
				header = Marshal.PtrToStructure<FullToc.FullTocHeader>((IntPtr)buffPP);

				int length = header.LengthLo | header.LengthHi << 8;

				byte* buffP = buffPP + Marshal.SizeOf<FullToc.FullTocHeader>();
				byte* buffEndP = buffP + (length - 2);
				for(var trackIndex = 0; buffP < buffEndP; ++trackIndex) {
					var entry = Marshal.PtrToStructure<CdTextDataBlock.Native>((IntPtr)buffP);

					blocks.Add(entry.AsManaged());

					buffP += Marshal.SizeOf<CdTextDataBlock.Native>();
				}
			}

			return blocks;
		}

		public CDText? GetCdText(int session=1) {
			try {
				var blocks = GetCdTextBlocks(session);

				return CDText.FromBlocks(blocks);
			} catch(Win32Exception err) when ((uint)err.HResult==0x80004005) {
				return null;
			}
		}

		[SecuritySafeCritical]
		public unsafe string? GetMediaCatalogNumber() {
			SubQDataFormat dataFormat = new SubQDataFormat() {
				Format = (byte)SubQDataFormatFormat.MediaCatalog,
				Track = 0
			};

			var catNr = file.DeviceControlInputOutput<SubQDataFormat, SubQMediaCatalogNumber>(DeviceIoControlCode.CdRomReadQChannel, ref dataFormat);

			if((catNr.ReservedMcVal & 128) == 0) {
				return null;
			}


			return new string((sbyte*)catNr.MediaCatalog);
		}

		[SecuritySafeCritical]
		public unsafe string? GetTrackISRC(byte track) {
			SubQDataFormat dataFormat = new SubQDataFormat() {
				Format = (byte)SubQDataFormatFormat.TrackISRC,
				Track = track
			};

			var isrc = file.DeviceControlInputOutput<SubQDataFormat, SubQTrackISRC>(DeviceIoControlCode.CdRomReadQChannel, ref dataFormat);

			if((isrc.ReservedTcVal & 128) == 0) {
				return null;
			}

			return new string((sbyte*)isrc.TrackIsrc);
		}

		[SecuritySafeCritical]
		public RegionData GetRegionData() {
			RegionData.Native native = new RegionData.Native();
			file.DeviceControlOutput<RegionData.Native>(DeviceIoControlCode.DvdGetRegion, ref native);
			return native.AsManaged();
		}

		[SecuritySafeCritical]
		public string GetMountPoint() {
			StorageDeviceNumber native=new StorageDeviceNumber();

			file.DeviceControlOutput(DeviceIoControlCode.StorageGetDeviceNumber, ref native);

			return native.GetDeviceName();
		}

		[SecuritySafeCritical]
		public SessionData GetSessionData() {
			SessionData.Native native=new SessionData.Native();
			file.DeviceControlOutput<SessionData.Native>(DeviceIoControlCode.CdRomGetLastSession, ref native);
			return native.AsManaged();
		}

		[SecuritySafeCritical]
		public void RawRead(UInt64 diskOffset, UInt32 sectorCount, TrackReadMode readMode, byte[] buffer) {
			RawReadInfo info = new RawReadInfo() {
				DiskOffset = diskOffset,
				SectorCount = sectorCount,
				ReadMode = readMode
			};
			RawRead(info, buffer);
		}

		private void RawRead(RawReadInfo info, byte[] buffer) {
			file.DeviceControlInputOutput(DeviceIoControlCode.CdRomRawRead, ref info, buffer);
		}

		public bool DiskIsReadOnly {
			get {
				try {
					file.DeviceControl(DeviceIoControlCode.DiskIsWriteable);
					return false;
				} catch (Win32Exception ex) when (ex.NativeErrorCode==19) {
					return true;
				}
			}
		}
	}
}
