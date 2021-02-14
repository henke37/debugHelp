using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.LastUnloadedModules {
	public class UnloadEventTrace {
		public IntPtr BaseAddress;   // Base address of dll
		public Int64 SizeOfImage;  // Size of image
		public UInt32 Sequence;      // Sequence number for this event
		public UInt32 TimeDateStamp; // Time and date of image
		public UInt32 CheckSum;      // Image checksum
		public string ImageName; // Image name

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal unsafe struct Native {
#if x86
			Int32 BaseAddress;   // Base address of dll
			Int32 SizeOfImage;  // Size of image
#elif x64
			Int64 BaseAddress;   // Base address of dll
			Int64 SizeOfImage;  // Size of image
#else
#error "Unknown struct layout!"
#endif
			UInt32 Sequence;      // Sequence number for this event
			UInt32 TimeDateStamp; // Time and date of image
			UInt32 CheckSum;      // Image checksum
			fixed char ImageName[32]; // Image name
#if x86
			fixed byte alignment[8];
#elif x64
			fixed byte alignment[12];
#endif

			public UnloadEventTrace AsManaged() {
				fixed (char *imgName=this.ImageName) {
					return new UnloadEventTrace() {
						BaseAddress = new IntPtr(BaseAddress),
						SizeOfImage = SizeOfImage,
						Sequence = Sequence,
						TimeDateStamp = TimeDateStamp,
						CheckSum = CheckSum,
						ImageName= new string(imgName)
					};
				}
			}
		}

		public override String ToString() {
			return $"{ImageName} 0x{BaseAddress:x}";
		}
	}
}
