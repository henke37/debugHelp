using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Henke37.Win32.Base;
using Henke37.Win32.Base.AccessRights;

namespace Henke37.Win32.CdAccess {
	public class CdDrive {
		internal NativeFileObject file;
		private const int TOCTrackCount=100;

		public CdDrive(string path) {
			file = NativeFileObject.Open(path, FileObjectAccessRights.GenericRead|FileObjectAccessRights.GenericWrite, FileShareMode.Write|FileShareMode.Read, FileDisposition.OpenExisting, 0);
		}

		public void Eject() {
			file.DeviceControl(DeviceIoControlCode.DiskEjectMedia);
		}

		public void Load() {
			file.DeviceControl(DeviceIoControlCode.DiskLoadMedia);
		}

		public unsafe TOC GetTOC() {
			int buffSize = Marshal.SizeOf<TOC.TocHeader>() + Marshal.SizeOf <TrackEntry.Native>() *TOCTrackCount;
			byte[] buff = new byte[buffSize];

			var tracks = new List<TrackEntry>();

			TOC.TocHeader header;

			file.DeviceControlOutput(DeviceIoControlCode.CdRomReadTOC, buff);
			fixed(byte* buffPP=buff) {
				header=Marshal.PtrToStructure<TOC.TocHeader>((IntPtr)buffPP);

				byte* buffP = buffPP;
				for(var trackIndex=0;trackIndex< header.LastTrack; ++trackIndex) {
					var entry=Marshal.PtrToStructure<TrackEntry.Native>((IntPtr)buffP);
					tracks.Add(entry.AsNative());

					buffP += Marshal.SizeOf<TrackEntry.Native>();
				}
			}

			return new TOC() {
				FirstTrack = header.FirstTrack,
				LastTrack = header.LastTrack,
				Tracks = tracks
			};
		}

		public RegionData GetRegionData() {
			RegionData.Native native=new RegionData.Native();
			file.DeviceControlOutput<RegionData.Native>(DeviceIoControlCode.DvdGetRegion, ref native);
			return native.AsManaged();
		}
	}
}
