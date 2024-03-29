﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

using Henke37.Win32.AccessRights;
using Henke37.Win32.DeviceEnum;
using Henke37.Win32.Files;
using static Henke37.Win32.CdAccess.Configuration;

namespace Henke37.Win32.CdAccess {
	public class CdDrive {
		internal NativeFileObject file;
		private const int TOCTrackCount = 100;

		private const int STATUS_BUFFER_OVERFLOW = unchecked((int)0x80000005);

		private readonly static Guid CdDriveGuid = new Guid("53F56308-B6BF-11D0-94F2-00A0C91EFB8B");

		private CdDrive(string path) {
			file = NativeFileObject.Open(path, FileObjectAccessRights.GenericRead | FileObjectAccessRights.GenericWrite, FileShareMode.Write | FileShareMode.Read, FileDisposition.OpenExisting, 0);
		}

		public CdDrive(DeviceInterface deviceInterface) : this(deviceInterface.FilePath) { }

		public static IEnumerable<DeviceInterface> GetCdDrives() {
			var devInfo = new DeviceInformationSet(CdDriveGuid, DeviceInformationClassFlags.Present | DeviceInformationClassFlags.DeviceInterface);
			return devInfo.GetDevices();
		}

		[SecuritySafeCritical]
		public void Eject() {
			file.DeviceControl(DeviceIoControlCode.DiskEjectMedia);
		}

		[SecuritySafeCritical]
		public void Load() {
			file.DeviceControl(DeviceIoControlCode.DiskLoadMedia);
		}

		[SecuritySafeCritical]
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
					ConfigurationHeader* header = (ConfigurationHeader*)buffPP;

					uint headersSize = (uint)sizeof(ConfigurationHeader);
					if(header->Length < headersSize) throw new Exception("Bad size!");

					byte* buffP = buffPP + sizeof(ConfigurationHeader);

					headersSize += (uint)sizeof(FeatureHeader);

					if(header->Length < headersSize) throw new Exception("Bad size!");

					FeatureHeader* featureHeader = (FeatureHeader*)buffP;

					if(featureHeader->Feature != feature) return null;

					if(header->Length < headersSize + featureHeader->AdditonalLength) throw new Exception("Bad size!");
					if(buffSize < headersSize + featureHeader->AdditonalLength) {
						buffSize = headersSize + featureHeader->AdditonalLength;
						continue;
					}

					var addData = buffP + sizeof(FeatureHeader);

					return FeatureDesc.BuffToDesc(*featureHeader, addData);
				}

			}
		}

		[SecuritySafeCritical]
		public unsafe CdLockToken Lock(string callerName, bool ignoreMount) {
			ExclusiveAccessLockRequest request = new ExclusiveAccessLockRequest() {
				Access = new ExcusiveAccessRequest() { 
					RequestType= ExclusiveAccessRequestType.LockDevice, 
					Flags = ignoreMount?ExclusiveAccessRequestFlags.IgnoreVolume:0
				},
			};

			if(callerName.Length > 63) throw new ArgumentOutOfRangeException(nameof(callerName), "Name is too long!");

			fixed(char* charsP= callerName.ToCharArray()) {
				Encoding.ASCII.GetBytes(
					charsP,
					callerName.Length,
					request.CallerName,
					64);
			}

			file.DeviceControlInput(DeviceIoControlCode.CdRomExclusiveAccess, ref request);

			return new CdLockToken(this);
		}

		[SecuritySafeCritical]
		public InqueryData GetInqueryData() {
			InqueryData.Native native=new InqueryData.Native();

			file.DeviceControlOutput(DeviceIoControlCode.CdRomGetInqueryData, ref native);

			return new InqueryData(native);
		}

		[SecuritySafeCritical]
		internal unsafe void Unlock(bool noNotifications) {
			ExcusiveAccessRequest request = new ExcusiveAccessRequest() {
				RequestType = ExclusiveAccessRequestType.UnlockDevice,
				Flags = noNotifications ? ExclusiveAccessRequestFlags.NoMediaNotifications : 0
			};
			file.DeviceControlInput(DeviceIoControlCode.CdRomExclusiveAccess, ref request);
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

		private void ReadSubcodeData(uint sectorNr, byte[] outBuff) {
			byte[] buff = new byte[(uint)RawReadSize.SectorWithSubcode];

			RawReadInfo info = new RawReadInfo() {
				ReadMode=TrackReadMode.RawWithSubCode,
				SectorCount=1
			};

			info.DiskOffset = 2048 * sectorNr;
			RawRead(info, buff);

			//trim off the userdata
			Array.Copy(buff, 2352, outBuff, 0, 96);
		}

		private static void ParseSubcode(byte[] inBuff, int subcodeIndex, byte[] outBuff) {
			Array.Clear(outBuff, 0, 12);
			int bitMask = 1 << subcodeIndex;
			for(int i = 0;i < 96; i++) {
				bool subBit = (inBuff[i] & bitMask) == bitMask;
				outBuff[i / 8] |= (byte)(subBit?(1 << (8-(i%8))):0);
			}
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

		public string DeviceName => GetDeviceName();

		[SecuritySafeCritical]
		internal string GetDeviceName() {

			int len = 200;

			for(; ; ) {
				try {
					var buff = new byte[len];

					try {
						file.DeviceControlOutput(DeviceIoControlCode.MountDevQueryDeviceName, buff);
					} finally {
						len = buff[0] | (buff[1] << 8);
					}

					return Encoding.Unicode.GetString(buff, 2, len);
				} catch(Win32Exception err) when(err.NativeErrorCode == STATUS_BUFFER_OVERFLOW) {
					//nop
				}
			}
		}
	}
}
