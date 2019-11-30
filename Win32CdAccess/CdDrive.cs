using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Henke37.Win32.Base;
using Henke37.Win32.Base.AccessRights;
using Henke37.Win32.DeviceEnum;

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

		public unsafe FullToc GetFullTOC() {
			ReadTocEx readToc = new ReadTocEx(ReadTocFormat.FullToc, true);

			int buffSize = Marshal.SizeOf<FullToc.FullTocHeader>() + Marshal.SizeOf<TocFullDataBlock.Native>() * TOCTrackCount;
			byte[] buff = new byte[buffSize];

			var tracks = new List<TocFullDataBlock>();

			FullToc.FullTocHeader header;

			file.DeviceControlInputOutput(DeviceIoControlCode.CdRomReadTOC, ref readToc, buff);
			fixed(byte* buffPP = buff) {
				header = Marshal.PtrToStructure<FullToc.FullTocHeader>((IntPtr)buffPP);

				byte* buffP = buffPP + Marshal.SizeOf<FullToc.FullTocHeader>();
				byte* buffEndP = buffP + (header.Length - 2);
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

		public RegionData GetRegionData() {
			RegionData.Native native = new RegionData.Native();
			file.DeviceControlOutput<RegionData.Native>(DeviceIoControlCode.DvdGetRegion, ref native);
			return native.AsManaged();
		}

		public string GetMountPoint() {
			StorageDeviceNumber native=new StorageDeviceNumber();

			file.DeviceControlOutput(DeviceIoControlCode.StorageGetDeviceNumber, ref native);

			return native.GetDeviceName();
		}

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
	}
}
