using Henke37.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.Jobs {
	public class ExtendedLimitInformation : BasicLimitInformation {
#if x64
		public UInt64 ProcessMemoryLimit;
		public UInt64 JobMemoryLimit;
		public UInt64 PeakProcessMemoryUsed;
		public UInt64 PeakJobMemoryUsed;
#elif x86
		public UInt32 ProcessMemoryLimit;
		public UInt32 JobMemoryLimit;
		public UInt32 PeakProcessMemoryUsed;
		public UInt32 PeakJobMemoryUsed;
#endif

		public ExtendedLimitInformation() { }
		internal ExtendedLimitInformation(Native native) : base(native.basic) {
			ProcessMemoryLimit = native.ProcessMemoryLimit;
			JobMemoryLimit = native.JobMemoryLimit;
			PeakProcessMemoryUsed = native.PeakProcessMemoryUsed;
			PeakJobMemoryUsed = native.PeakJobMemoryUsed;
		}

		[StructLayout(LayoutKind.Sequential)]
		new internal struct Native {
			internal BasicLimitInformation.Native basic;

			IOCounters IOCounters;
#if x64
			public UInt64 ProcessMemoryLimit;
			public UInt64 JobMemoryLimit;
			public UInt64 PeakProcessMemoryUsed;
			public UInt64 PeakJobMemoryUsed;
#elif x86
			public UInt32 ProcessMemoryLimit;
			public UInt32 JobMemoryLimit;
			public UInt32 PeakProcessMemoryUsed;
			public UInt32 PeakJobMemoryUsed;
#endif

			public Native(ExtendedLimitInformation managed) {
				basic = new BasicLimitInformation.Native(managed);

				IOCounters = new IOCounters(0);
				ProcessMemoryLimit = managed.ProcessMemoryLimit;
				JobMemoryLimit = managed.JobMemoryLimit;
				PeakProcessMemoryUsed = managed.PeakProcessMemoryUsed;
				PeakJobMemoryUsed = managed.PeakJobMemoryUsed;
			}

			public ExtendedLimitInformation AsManaged() {
				return new ExtendedLimitInformation(this);
			}
		}
	}
}
