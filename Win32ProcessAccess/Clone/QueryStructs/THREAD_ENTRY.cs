using Henke37.Win32.Base;
using Henke37.Win32.Processes;
using System;
using System.Runtime.InteropServices;

using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Henke37.Win32.Clone.QueryStructs {
	public class ThreadEntry {

		public UInt32 ExitStatus;
		public UIntPtr TebBaseAddress;
		public UInt32 ProcessId;
		public UInt32 ThreadId;
		public UIntPtr AffinityMask;
		public int Priority;
		public int BasePriority;
		DateTime CreateTime;
		DateTime ExitTime;
		TimeSpan KernelTime;
		TimeSpan UserTime;
		public UIntPtr Win32StartAddress;
		DateTime CaptureTime;
		public bool Terminated;
		public UInt16 SuspendCount;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal unsafe struct Native {
			UInt32 ExitStatus;
			UIntPtr TebBaseAddress;
			UInt32 ProcessId;
			UInt32 ThreadId;
			UIntPtr AffinityMask;
			int Priority;
			int BasePriority;
			UIntPtr LastSyscallFirstArgument;
			UInt16 LastSyscallNumber;
			FILETIME CreateTime;
			FILETIME ExitTime;
			FILETIME KernelTime;
			FILETIME UserTime;
			UIntPtr Win32StartAddress;
			FILETIME CaptureTime;
			THREAD_FLAGS Flags;
			UInt16 SuspendCount;
			UInt16 SizeOfContextRecord;
			void* ContextRecord;

			internal ThreadEntry AsManaged() {
				return new ThreadEntry() { 
					ExitStatus = ExitStatus,
					TebBaseAddress = TebBaseAddress,
					ProcessId = ProcessId,
					ThreadId = ThreadId,
					AffinityMask = AffinityMask,
					Priority = Priority,
					BasePriority = BasePriority,
					CreateTime = CreateTime.ToDateTime(),
					ExitTime = ExitTime.ToDateTime(),
					KernelTime = KernelTime.ToTimeSpan(),
					UserTime = UserTime.ToTimeSpan(),
					Win32StartAddress = Win32StartAddress,
					CaptureTime = CaptureTime.ToDateTime(),
					Terminated = (Flags & THREAD_FLAGS.Terminated) != 0,
					SuspendCount = SuspendCount
				};
			}
		}
	}

	enum THREAD_FLAGS {
		None = 0,
		Terminated = 1
	}
}
