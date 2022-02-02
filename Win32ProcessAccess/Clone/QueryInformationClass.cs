using System;

namespace Henke37.Win32.Clone {
	internal enum QueryInformationClass : UInt32 {
        PROCESS_INFORMATION = 0,
        VA_CLONE_INFORMATION = 1,
        AUXILIARY_PAGES_INFORMATION = 2,
        VA_SPACE_INFORMATION = 3,
        HANDLE_INFORMATION = 4,
        THREAD_INFORMATION = 5,
        HANDLE_TRACE_INFORMATION = 6,
        PERFORMANCE_COUNTERS = 7
    }
}
