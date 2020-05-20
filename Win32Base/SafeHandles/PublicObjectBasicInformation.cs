using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Base.SafeHandles {
	public class PublicObjectBasicInformation {
		public UInt32 Attributes;
		public UInt32 GrantedAccess;
		public UInt32 HandleCount;
		public UInt32 PointerCount;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal unsafe struct Native {
			UInt32 Attributes;
			UInt32 GrantedAccess;
			UInt32 HandleCount;
			UInt32 PointerCount;
			fixed UInt32 Reserved[10];

			public PublicObjectBasicInformation AsManaged() {
				return new PublicObjectBasicInformation() {
					Attributes    = Attributes,
					GrantedAccess = GrantedAccess,
					HandleCount   = HandleCount,
					PointerCount  = PointerCount
				};
			}
		}
	}
}
