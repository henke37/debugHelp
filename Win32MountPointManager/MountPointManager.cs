using Henke37.Win32.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Henke37.Win32.MountPointManager {
	[SuppressUnmanagedCodeSecurity]
	public class MountPointManager {
		private NativeFileObject file;

		public MountPointManager() {
			file = NativeFileObject.Open(
				"\\\\.\\MountPointManager", 
				AccessRights.FileObjectAccessRights.None,
				FileShareMode.Read,
				FileDisposition.OpenExisting, 
				FileAttributes.Normal
			);
		}

		public unsafe List<MountPoint> QueryPoints(MountPoint needle) {
			var buff=needle.ToBuff();

			return QueryPoints(buff);
		}

		public List<MountPoint> QueryAllPoints() {
			var dummy = new MountPoint();
			return QueryPoints(dummy.ToBuff());
		}

		private unsafe List<MountPoint> QueryPoints(byte[] inBuff) {

			uint buffSize = 42;
			for(; ; ) {
				byte[] outBuff = new byte[buffSize];

				try {
					file.DeviceControlInputOutput(DeviceIoControlCode.MountMgrQueryPoints, inBuff, outBuff);

					var list = new List<MountPoint>();

					fixed(byte* outBuffP = outBuff) {
						var hdr = (MountPointListHeader*)outBuffP;

						var entryHdr = (MountPoint.Header*)(hdr + 1);
						for(uint MountPointIndex=0;MountPointIndex<hdr->NumberOfMountPoints;++MountPointIndex,++entryHdr) {
							var entry = new MountPoint();

							if(entryHdr->SymLinkNameOffset!=0) {
								var strPtr = outBuffP + entryHdr->SymLinkNameOffset;
								entry.SymbolicLinkName = Encoding.Unicode.GetString(strPtr, entryHdr->SymLinkNameLength);
							}

							if(entryHdr->DeviceNameOffset != 0) {
								var strPtr = outBuffP + entryHdr->DeviceNameOffset;
								entry.DeviceName = Encoding.Unicode.GetString(strPtr, entryHdr->DeviceNameLength);
							}

							if(entryHdr->UniqueIdOffset!=0) {
								var strPtr = outBuffP + entryHdr->UniqueIdOffset;

								int tests = 0x0003;
								if(IsTextUnicode(strPtr, entryHdr->UniqueIdLength, ref tests)) {
									entry.UniqueId = Encoding.Unicode.GetString(strPtr, entryHdr->UniqueIdLength);
								} else {
									entry.UniqueId = GetHexString(strPtr, entryHdr->UniqueIdLength);
								}
							}

							list.Add(entry);
						}
					}

					return list;
				} catch(Win32Exception err) when(err.NativeErrorCode == 234) {
					//reallocate buffer with correct size
					fixed(byte* outBuffP = outBuff) {
						var hdr = (MountPointListHeader*)outBuffP;
						buffSize = hdr->Size;
					}
				}
			}

			
		}

		private static unsafe string GetHexString(byte* strPtr, int byteCnt) {
			var sb = new StringBuilder("0x", byteCnt*2+2);
			for(uint byteIndex = 0; byteIndex < byteCnt; ++byteIndex) {
				int val = strPtr[byteIndex] & 0x0F;
				sb.Append((val > 9) ? 'A' + (val - 10) : '0' + val);
				val = (strPtr[byteIndex] >> 4) & 0x0F;
				sb.Append((val > 9) ? 'A' + (val - 10) : '0' + val);
			}
			return sb.ToString();
		}

		[DllImport("Advapi32.dll", ExactSpelling = true, SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool IsTextUnicode(byte* buff, int buffLen, ref int tests);

	}
}
