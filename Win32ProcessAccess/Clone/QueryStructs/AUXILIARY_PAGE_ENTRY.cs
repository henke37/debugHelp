using Henke37.Win32.Base;
using Henke37.Win32.Memory;
using Henke37.Win32.Processes;
using System;
using System.Runtime.InteropServices;

using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Henke37.Win32.Clone.QueryStructs {
	unsafe class AuxiliaryPageEntry {
		public UIntPtr Address;
		public MemoryBasicInformation BasicInformation;
		public DateTime CaptureTime;
		public void* PageContents;
		public UInt32 PageSize;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal unsafe struct Native {
			UIntPtr Address;
			MemoryBasicInformation.Native BasicInformation;
			FILETIME CaptureTime;
			void* PageContents;
			UInt32 PageSize;

			public AuxiliaryPageEntry AsManaged() {
				return new AuxiliaryPageEntry() {
					Address = Address,
					BasicInformation = BasicInformation.AsManaged(),
					CaptureTime = CaptureTime.ToDateTime(),
					PageContents = PageContents,
					PageSize = PageSize
				};
			}
		}
	}
}
