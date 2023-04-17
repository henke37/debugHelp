using Henke37.Win32.Base;
using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.SafeHandles {
	[Undocumented]
	public class PublicObjectBasicInformation {
		public ObjectAttributes Attributes;
		public UInt32 GrantedAccess;
		public UInt32 HandleCount;
		public UInt32 PointerCount;

		[Undocumented]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal unsafe struct Native {
            UInt32 Attributes;
			UInt32 GrantedAccess;
			UInt32 HandleCount;
			UInt32 PointerCount;
			fixed UInt32 Reserved[10];

			public PublicObjectBasicInformation AsManaged() {
				return new PublicObjectBasicInformation() {
					Attributes    = ((ObjectAttributes)Attributes),
					GrantedAccess = GrantedAccess,
					HandleCount   = HandleCount,
					PointerCount  = PointerCount
				};
			}
		}
	}

    [Undocumented]
    public enum ObjectAttributes : UInt32
    {
        None = 0,
        Inherit = 0x0002,
		Permanent = 0x0010,
        Exclusive = 0x0020,
		CaseInsensitive = 0x0040,
		OpenIf = 0x0080,
		OpenLink = 0x0100,
        Kernel = 0x0200,
		ForceAccessCheck = 0x0400
	}
}
