using System;

namespace Henke37.Win32.Clone {
	[Flags]
	public enum CloneFlags : UInt32 {
        None = 0x00000000,
        VAClone = 0x00000001,
        RESERVED_00000002 = 0x00000002,
        Handles = 0x00000004,
        HandleNameInformation = 0x00000008,
        HandleBasicInformation = 0x00000010,
        HandleTypeSpecificInformation = 0x00000020,
        HandleTracing = 0x00000040,
        Threads = 0x00000080,
        ThreadContexts = 0x00000100,
        ThreadExtendedContexts = 0x00000200,
        RESERVED_00000400 = 0x00000400,
        VASpace = 0x00000800,
        VASpaceMappedSectionInformation = 0x00001000,

        BreakawayOptional = 0x04000000,
        Breakaway = 0x08000000,
        BreakawayForced = 0x10000000,
        UseVMAllocations = 0x20000000,
        MeasurePerformance = 0x40000000,
        ReleaseImageSection = 0x80000000
    }
}
