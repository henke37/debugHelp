using System;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Tokens {
	[StructLayout(LayoutKind.Sequential)]
	public struct TokenStatistics {
		public UInt64 TokenId;
		public UInt64 AuthenticationId;
		public LargeInteger ExpirationTime;
		public ImpersonationLevel ImpersonationLevel;
		public UInt32 DynamicCharged;
		public UInt32 DynamicAvailable;
		public UInt32 GroupCount;
		public UInt32 PrivilegeCount;
		public UInt64 ModifiedId;
	}
}
