using System.Text;

namespace Henke37.Win32.MountPointManager {
	public class MountPoint {
		public string? SymbolicLinkName;
		public string? UniqueId;
		public string? DeviceName;

		internal unsafe byte[] ToBuff() {
			var ms = new MemoryStream();

			using(var w=new BinaryWriter(ms,Encoding.Unicode, true)) {
				int linkOffset=0;
				int uidOffset=0;
				int devNameOffset=0;
				w.Seek(sizeof(Header),SeekOrigin.Begin);
				if(SymbolicLinkName!= null) {
					linkOffset = (int)ms.Position;
					w.Write(SymbolicLinkName.ToCharArray());
				}
				if(UniqueId!= null) {
					uidOffset = (int)ms.Position;
					w.Write(UniqueId.ToCharArray());
				}
				if(DeviceName!= null) {
					devNameOffset = (int)ms.Position;
					w.Write(DeviceName.ToCharArray());
				}

				w.Seek(0, SeekOrigin.Begin);
				w.Write(linkOffset);
				w.Write(SymbolicLinkName?.Length??0);
				w.Seek(2, SeekOrigin.Current);
				w.Write(uidOffset);
				w.Write(UniqueId?.Length ?? 0);
				w.Seek(2, SeekOrigin.Current);
				w.Write(devNameOffset);
				w.Write(DeviceName?.Length ?? 0);
				w.Seek(2, SeekOrigin.Current);

			}

			return ms.GetBuffer();
		}

		internal struct Header {
			internal UInt32 SymLinkNameOffset;
			internal UInt16 SymLinkNameLength;
			internal UInt16 Padding1;

			internal UInt32 UniqueIdOffset;
			internal UInt16 UniqueIdLength;
			internal UInt16 Padding2;

			internal UInt32 DeviceNameOffset;
			internal UInt16 DeviceNameLength;
			internal UInt16 Padding3;
		}
	}
}
