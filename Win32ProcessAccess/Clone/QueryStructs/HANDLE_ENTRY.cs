using Henke37.Win32.Base;
using Henke37.Win32.Processes;
using System;
using System.Runtime.InteropServices;

using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Henke37.Win32.Clone.QueryStructs {
	public class HandleEntry {
		public IntPtr Handle;
		public HandleFlag Flags;
		public ObjectType ObjectType;
		public DateTime CaptureTime;
		public UInt32 Attributes;
		public UInt32 GrantedAccess;
		public UInt32 HandleCount;
		public UInt32 PointerCount;
		public UInt32 PagedPoolCharge;
		public UInt32 NonPagedPoolCharge;
		public DateTime CreationTime;
		public string? TypeName;
		public string? ObjectName;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal unsafe struct Native {
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

			ProcessHandleEntry.Native Process;
			ThreadHandleEntry.Native Thread;
			MutantHandleEntry.Native Mutant;
			EventHandleEntry.Native Event;
			SectionHandleEntry.Native Section;
			SemaphoreHandleEntry.Native Semaphore;

			internal HandleEntry AsManaged() {
				HandleEntry entry;
				if((Flags & HandleFlag.HaveTypeSpecificInformation)!=0) {
					switch(ObjectType) {
						case ObjectType.Process:
							entry = Process.AsManaged();
							break;
						case ObjectType.Thread:
							entry = Thread.AsManaged();
							break;
						case ObjectType.Mutant:
							entry = Mutant.AsManaged();
							break;
						case ObjectType.Event:
							entry = Event.AsManaged();
							break;
						case ObjectType.Section:
							entry = Section.AsManaged();
							break;
						case ObjectType.Semaphore:
							entry = Semaphore.AsManaged();
							break;

						case ObjectType.Unknown:
							entry = new HandleEntry();
							break;
						default:
							throw new NotImplementedException();
					}
				} else {
					entry = new HandleEntry();
				}

				entry.Handle = Handle;
				entry.Flags = Flags;

				if((Flags & HandleFlag.HaveBasicInformation) != 0) {
					entry.CaptureTime = CaptureTime.ToDateTime();
					entry.Attributes = Attributes;
					entry.GrantedAccess = GrantedAccess;
					entry.HandleCount = HandleCount;
					entry.PointerCount = PointerCount;
					entry.PagedPoolCharge = PagedPoolCharge;
					entry.NonPagedPoolCharge = NonPagedPoolCharge;
					entry.CreationTime = CreationTime.ToDateTime();
				}

				if((Flags & HandleFlag.HaveType)!=0) {
					entry.TypeName = new string(TypeName, 0, (int)TypeNameLength);
				}
				if((Flags & HandleFlag.HaveName) != 0) {
					entry.TypeName = new string(ObjectName, 0, (int)ObjectNameLength);
				}

				return entry;
			}
		}
	}

	public class ProcessHandleEntry : HandleEntry {
		public UInt32 ExitStatus;
		public IntPtr PebBaseAddress;
		public IntPtr AffinityMask;
		public UInt32 BasePriority;
		public UInt32 ProcessId;
		public UInt32 ParentProcessId;
		public ProcessFlag ProcessFlags;

		internal new struct Native {
			UInt32 ExitStatus;
			IntPtr PebBaseAddress;
			IntPtr AffinityMask;
			UInt32 BasePriority;
			UInt32 ProcessId;
			UInt32 ParentProcessId;
			ProcessFlag Flags;

			internal ProcessHandleEntry AsManaged() {
				return new ProcessHandleEntry() {
					ExitStatus = ExitStatus,
					PebBaseAddress = PebBaseAddress,
					AffinityMask = AffinityMask,
					BasePriority = BasePriority,
					ProcessId = ProcessId,
					ParentProcessId = ParentProcessId,
					ProcessFlags = Flags
				};
			}
		}
	}

	public class ThreadHandleEntry : HandleEntry {
		public UInt32 ExitStatus;
		public IntPtr TebBaseAddress;
		public UInt32 ProcessId;
		public UInt32 ThreadId;
		public IntPtr AffinityMask;
		public int Priority;
		public int BasePriority;
		public IntPtr Win32StartAddress;

		internal new struct Native {
			UInt32 ExitStatus;
			IntPtr TebBaseAddress;
			UInt32 ProcessId;
			UInt32 ThreadId;
			IntPtr AffinityMask;
			int Priority;
			int BasePriority;
			IntPtr Win32StartAddress;

			internal ThreadHandleEntry AsManaged() {
				return new ThreadHandleEntry() {
					ExitStatus = ExitStatus,
					TebBaseAddress = TebBaseAddress,
					ProcessId = ProcessId,
					ThreadId = ThreadId,
					AffinityMask = AffinityMask,
					Priority = Priority,
					BasePriority = BasePriority,
					Win32StartAddress = Win32StartAddress
				};
			}
		}

	}

	public class MutantHandleEntry : HandleEntry {
		public Int32 CurrentCount;
		public bool Abandoned;
		public UInt32 OwnerProcessId;
		public UInt32 OwnerThreadId;

		internal new struct Native {
			Int32 CurrentCount;
			UInt32 Abandoned;
			UInt32 OwnerProcessId;
			UInt32 OwnerThreadId;

			internal MutantHandleEntry AsManaged() {
				return new MutantHandleEntry() {
					CurrentCount = CurrentCount,
					Abandoned = Abandoned != 0,
					OwnerProcessId = OwnerProcessId,
					OwnerThreadId = OwnerThreadId
				};
			}
		}
	}
	public class EventHandleEntry : HandleEntry {
		public bool ManualReset;
		public bool Signaled;

		internal new struct Native {
			UInt32 ManualReset;
			UInt32 Signaled;

			internal EventHandleEntry AsManaged() {
				return new EventHandleEntry() {
					ManualReset = ManualReset!=0,
					Signaled = Signaled!=0
				};
			}
		}
	}
	public class SectionHandleEntry : HandleEntry {
		public IntPtr BaseAddress;
		public UInt32 AllocationAttributes;
		public LargeInteger MaximumSize;

		internal new struct Native {
			IntPtr BaseAddress;
			UInt32 AllocationAttributes;
			LargeInteger MaximumSize;

			internal SectionHandleEntry AsManaged() {
				return new SectionHandleEntry() {
					BaseAddress = BaseAddress,
					AllocationAttributes = AllocationAttributes,
					MaximumSize = MaximumSize
				};
			}
		}
	}
	public class SemaphoreHandleEntry : HandleEntry {
		public Int32 CurrentCount;
		public Int32 MaximumCount;

		internal new struct Native {
			Int32 CurrentCount;
			Int32 MaximumCount;

			internal SemaphoreHandleEntry AsManaged() {
				return new SemaphoreHandleEntry() {
					CurrentCount = CurrentCount,
					MaximumCount = MaximumCount
				};
			}
		}
	}

	[Flags]
	public enum HandleFlag {
		None = 0x00,
		HaveType = 0x01,
		HaveName = 0x02,
		HaveBasicInformation = 0x04,
		HaveTypeSpecificInformation = 0x08
	}

	public enum ObjectType {
		Unknown = 0,
		Process = 1,
		Thread = 2,
		Mutant = 3,
		Event = 4,
		Section = 5,
		Semaphore = 6
	}

	[Flags]
	public enum ProcessFlag : UInt32 {
		None = 0x00000000,
		Protected = 0x00000001,
		WOW64 = 0x00000002,
		RESERVED_03 = 0x00000004,
		RESERVED_04 = 0x00000008,
		Frozen = 0x00000010
	}
}
