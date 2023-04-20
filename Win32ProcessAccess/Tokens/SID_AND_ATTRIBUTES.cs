using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Henke37.Win32.Tokens {
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal unsafe struct SID_AND_ATTRIBUTES {
        internal IntPtr pSid;
        internal UInt32 dwAttributes;
	}

    public struct GroupEntry {
        [DebuggerDisplay("{SID} {Flags}")]
        public SecurityIdentifier SID;
        public GroupAttributeFlags Flags;
    }

    [Flags]
    public enum GroupAttributeFlags : UInt32 {
        None = 0,
        Enabled = 0x00000004,
        EnabledByDefault = 0x00000002,
        Integrity = 0x00000020,
        IntegrityEnabled = 0x00000040,
        LogonID = 0xC0000000,
        Mandatory = 0x00000001,
        Owner = 0x00000008,
        Resource = 0x20000000,
        DenyOnly = 0x00000010
    }
}