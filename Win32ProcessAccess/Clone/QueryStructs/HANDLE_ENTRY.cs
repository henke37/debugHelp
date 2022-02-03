using System;
using System.Runtime.InteropServices;

using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Henke37.Win32.Clone.QueryStructs {
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	unsafe struct HANDLE_ENTRY {
		IntPtr Handle;
		HandleFlag Flags;
		ObjectType ObjectType;
		FILETIME CaptureTime;
		UInt32 Attributes;
		UInt32 GrantedAccess;
		UInt32 HandleCount;
		UInt32 PointerCount;
		UInt32 PagedPoolCharge;
		UInt32 NonPagedPoolCharge;
		FILETIME CreationTime;
		UInt32 TypeNameLength;
		char* TypeName;
		UInt32 ObjectNameLength;
		char* ObjectName;

		struct ProcessHandle {
			UInt32 ExitStatus;
			IntPtr PebBaseAddress;
			IntPtr AffinityMask;
			UInt32 BasePriority;
			UInt32 ProcessId;
			UInt32 ParentProcessId;
			ProcessFlag Flags;
		}

		struct ThreadHandle {
			UInt32 ExitStatus;
			IntPtr TebBaseAddress;
			UInt32 ProcessId;
			UInt32 ThreadId;
			IntPtr AffinityMask;
			int Priority;
			int BasePriority;
			IntPtr Win32StartAddress;
		}

		struct Mutant {
			Int32 CurrentCount;
			UInt32 Abandoned;
			UInt32 OwnerProcessId;
			UInt32 OwnerThreadId;
		}
		struct Event {
			UInt32 ManualReset;
			UInt32 Signaled;
		}
		struct Section {
			IntPtr BaseAddress;
			UInt32 AllocationAttributes;
			LargeInteger MaximumSize;
		}
		struct Semaphore {
			Int32 CurrentCount;
			Int32 MaximumCount;
		}
	}

	[Flags]
	enum HandleFlag {
		None = 0x00,
		HaveType = 0x01,
		HaveName = 0x02,
		HaveBasicInformation = 0x04,
		HaveTypeSpecificInformation = 0x08
	}

	enum ObjectType {
		Unknown = 0,
		Process = 1,
		Thread = 2,
		Mutant = 3,
		Event = 4,
		Section = 5,
		Semaphore = 6
	}

	[Flags]
	enum ProcessFlag : UInt32 {
		None = 0x00000000,
		Protected = 0x00000001,
		WOW64 = 0x00000002,
		RESERVED_03 = 0x00000004,
		RESERVED_04 = 0x00000008,
		Frozen = 0x00000010
	}
}
