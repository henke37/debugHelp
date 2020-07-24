using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.Tokens {
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
