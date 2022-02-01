using System;

namespace Henke37.Win32.Clone {
	[Flags]
	public enum CloneFlags : UInt32 {
        None = 0x00000000,
        VA_Clone = 0x00000001,
        RESERVED_00000002 = 0x00000002,
        Handles = 0x00000004,
        Handle_NAME_INFORMATION = 0x00000008,
        Handle_BASIC_INFORMATION = 0x00000010,
        Handle_TYPE_SPECIFIC_INFORMATION = 0x00000020,
        Handle_TRACE = 0x00000040,
        Threads = 0x00000080,
        ThreadContexts = 0x00000100,
        ThreadContexts_EXTENDED = 0x00000200,
        RESERVED_00000400 = 0x00000400,
        VA_SPACE = 0x00000800,
        VA_SPACE_SECTION_INFORMATION = 0x00001000,

        BREAKAWAY_OPTIONAL = 0x04000000,
        BREAKAWAY = 0x08000000,
        FORCE_BREAKAWAY = 0x10000000,
        USE_VM_ALLOCATIONS = 0x20000000,
        MEASURE_PERFORMANCE = 0x40000000,
        RELEASE_SECTION = 0x80000000
    }
}
