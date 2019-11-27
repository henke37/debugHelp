using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Henke37.Win32.Base;
using Henke37.Win32.Base.AccessRights;

namespace Henke37.Win32.CdAccess {
	public class CdDrive {
		internal NativeFileObject file;
		private const int TOCTrackCount = 100;

		public CdDrive(string path) {
			file = NativeFileObject.Open(path, FileObjectAccessRights.GenericRead | FileObjectAccessRights.GenericWrite, FileShareMode.Write | FileShareMode.Read, FileDisposition.OpenExisting, 0);
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

				byte* buffP = buffPP;
				for(var trackIndex = 0; trackIndex < header.LastTrack; ++trackIndex) {
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

		public unsafe string? GetMediaCatalogNumber() {
			SubQDataFormat dataFormat = new SubQDataFormat() {
				Format = (byte)SubQDataFormatFormat.MediaCatalog,
				Track = 0
			};

			var catNr = file.DeviceControlInputOutput<SubQDataFormat, SubQMediaCatalogNumber>(DeviceIoControlCode.CdRomReadQChannel, ref dataFormat);

			if((catNr.ReservedMcVal & 128) == 0) {
				return null;
			}

			byte* b = catNr.MediaCatalog;
			return ReadStrZ(catNr.MediaCatalog);
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

			return ReadStrZ(isrc.TrackIsrc);
		}

		private static unsafe string ReadStrZ(byte* b) {
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < 15; i++) {
				char c = (char)b[i];
				if(c == 0) break;
				sb.Append(c);
			}

			return sb.ToString();
		}

		public RegionData GetRegionData() {
			RegionData.Native native = new RegionData.Native();
			file.DeviceControlOutput<RegionData.Native>(DeviceIoControlCode.DvdGetRegion, ref native);
			return native.AsManaged();
		}
	}
}
