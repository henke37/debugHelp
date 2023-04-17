using Henke37.Win32.AccessRights;
using Henke37.Win32.Processes;
using Henke37.Win32.SafeHandles;
using Henke37.Win32.Tokens;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

using System.Security.Principal;
using System.Threading;

namespace Henke37.Win32.Threads {

#if NETFRAMEWORK
	[HostProtection(ExternalThreading = true)]
#endif
	[SuppressUnmanagedCodeSecurity]
	public sealed class NativeThread : IDisposable {
		readonly SafeThreadHandle handle;

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static NativeThread Open(UInt32 threadId, ThreadAccessRights access = ThreadAccessRights.All, bool inheritable = false) {
			SafeThreadHandle handle = OpenThread((UInt32)access, inheritable, threadId);
			if(handle.IsInvalid) throw new Win32Exception();
			return new NativeThread(handle);
		}

		internal NativeThread(SafeThreadHandle handle) {
			this.handle = handle;
		}

#if NETFRAMEWORK
		[HostProtection(MayLeakOnAbort = true)]
#endif
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[ReliabilityContract(Consistency.MayCorruptProcess, Cer.None)]
		public NativeThread Reopen(ThreadAccessRights rights = ThreadAccessRights.All, bool inheritable = false) {
			var rawHandle = SafeKernelObjHandle.DuplicateHandleLocal(handle.DangerousGetHandle(), (uint)rights, inheritable, SafeKernelObjHandle.DuplicateOptions.None);
			return new NativeThread(new SafeThreadHandle(rawHandle));
		}

		public void Dispose() => Close();
		public void Close() {
			UnregisterExitedWait();
			handle.Close();
		}

		public UInt32 ThreadId {

			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				UInt32 threadId = GetThreadId(handle);
				if(threadId == 0) throw new Win32Exception();
				return threadId;
			}
		}

		public UInt32 ProcessId {

			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				UInt32 processId = GetProcessIdOfThread(handle);
				if(processId == 0) throw new Win32Exception();
				return processId;
			}
		}

		public unsafe UInt32 ExitCode {

			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				UInt32 exitCode;
				bool success = GetExitCodeThread(handle, &exitCode);
				if(!success) throw new Win32Exception();

				return exitCode;
			}
		}

		public unsafe string Description {

			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				char* descPtr;
				UInt32 status = GetThreadDescription(handle, &descPtr);
				if((status & 0x80000000) == 0x80000000) throw new Exception();
				string desc = new string(descPtr);
				Marshal.FreeHGlobal((IntPtr)descPtr);

				return desc;
			}
		}

#if x86
#if NETFRAMEWORK
		[HostProtection(ExternalThreading = true, Unrestricted = true)]
#endif
		public ThreadContext32 GetContext() {
			var ctx = new ThreadContext32();
			ctx.ReadFromHandle(handle);
			return ctx;
		}
#if NETFRAMEWORK
		[HostProtection(ExternalThreading = true, Unrestricted = true)]
#endif
		public void SetContext(ThreadContext32 context) {
			context.WriteToHandle(handle);
		}
#elif x64
#if NETFRAMEWORK
		[HostProtection(ExternalThreading=true, Unrestricted = true)]
#endif
		public ThreadContext64 GetContext() {
			var ctx = new ThreadContext64();
			ctx.ReadFromHandle(handle);
			return ctx;
		}
#if NETFRAMEWORK
		[HostProtection(ExternalThreading=true, Unrestricted = true)]
#endif
		public void SetContext(ThreadContext64 context) {
			context.WriteToHandle(handle);
		}
#if NETFRAMEWORK
		[HostProtection(ExternalThreading=true, Unrestricted = true)]
#endif
		public ThreadContext32 GetWow64Context() {
			var ctx = new ThreadContext32();
			ctx.ReadFromHandle(handle);
			return ctx;
		}
#if NETFRAMEWORK
		[HostProtection(ExternalThreading=true, Unrestricted = true)]
#endif
		public void SetWow64Context(ThreadContext32 context) {
			context.WriteToHandle(handle);
		}
#else
#error "No GetContext implementation"
#endif

#if x86
		[SecuritySafeCritical]
		public SelectorEntry GetSelector(UInt32 selector) {
			bool success = GetThreadSelectorEntry(handle, selector, out var native);
			if(!success) throw new Win32Exception();
			return native.AsManaged();
		}
#endif
#if x64
[SecuritySafeCritical]
		public SelectorEntry GetWow64Selector(UInt32 selector) {
			bool success = Wow64GetThreadSelectorEntry(handle, selector, out var native);
			if(!success) throw new Win32Exception();
			return native.AsManaged();
		}
#endif

		public ThreadPriorityLevel ThreadPriority {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var success = GetThreadPriority(handle, out var priority);
				if(!success) throw new Win32Exception();
				return (ThreadPriorityLevel)priority;
			}

			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			set {
				var success = SetThreadPriority(handle, (int)value);
				if(!success) throw new Win32Exception();
			}
		}

		public bool DynamicPriorityBoosts {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var success = GetThreadPriorityBoost(handle, out var disableBoosts);
				if(!success) throw new Win32Exception();
				return !disableBoosts;
			}

			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			set {
				bool disableBoost = !value;
				var success = SetThreadPriorityBoost(handle, disableBoost);
				if(!success) throw new Win32Exception();
			}
		}

		public bool IOPending {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var success = GetThreadIOPendingFlag(handle, out var flag);
				if(!success) throw new Win32Exception();
				return flag;
			}
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public ProcessTimes GetThreadTimes() {
			var success = GetThreadTimesNative(handle, out var creationTime, out var exitTime, out var kernelTime, out var userTime);
			if(!success) throw new Win32Exception();
			return new ProcessTimes(creationTime, exitTime, kernelTime, userTime);
		}

		private Action<NativeThread>? exitedListeners;
		private WaitHandle? exitedWaitHandle;
		private RegisteredWaitHandle? exitWaitRegistered;

		public event Action<NativeThread> Exited {
			add {
				if(exitedWaitHandle == null) {
					exitedWaitHandle = handle.MakeWaitHandle();
					exitedListeners = new Action<NativeThread>(value);
					exitWaitRegistered = ThreadPool.RegisterWaitForSingleObject(exitedWaitHandle, exitedCallback, null, -1, true);
				} else {
					exitedListeners += value;
				}
			}
			remove {
				exitedListeners -= value;

				if(exitedListeners!.GetInvocationList().Length == 0) {
					UnregisterExitedWait();
				}
			}
		}

		private void UnregisterExitedWait() {
			if(exitWaitRegistered == null) return;

			exitWaitRegistered!.Unregister(exitedWaitHandle);
			exitWaitRegistered = null;

			exitedWaitHandle!.Dispose();
			exitedWaitHandle = null;

			exitedListeners = null;
		}

		private void exitedCallback(object state, bool timedOut) {
			exitedListeners!.Invoke(this);
		}

		public NativeToken OpenToken(TokenAccessLevels accessLevels, bool ignoreImpersonation = false) {
			bool success = OpenThreadToken(handle, (uint)accessLevels, ignoreImpersonation, out SafeTokenHandle tokenHandle);
			if(!success) throw new Win32Exception();
			return new NativeToken(tokenHandle);
		}

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Suspend() {
			int suspendCount = SuspendThread(handle);
			if(suspendCount<0) throw new Win32Exception();
		}

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Resume() {
			int suspendCount = ResumeThread(handle);
			if(suspendCount < 0) throw new Win32Exception();
		}

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Terminate(UInt32 exitCode) {
			bool success = TerminateThread(handle, exitCode);
			if(!success) throw new Win32Exception();
		}

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void QueueUserAPC(IntPtr pfnAPC, IntPtr dwData, bool special = false) {
			bool success = QueueUserAPC2Native(pfnAPC, handle, dwData, special?QueueAPC2Flags.Special:QueueAPC2Flags.None);
			if(!success) throw new Win32Exception();
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern SafeThreadHandle OpenThread(UInt32 access, [MarshalAs(UnmanagedType.Bool)] bool inheritable, UInt32 threadId);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern UInt32 GetThreadId(SafeThreadHandle handle);
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern UInt32 GetProcessIdOfThread(SafeThreadHandle handle);
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool GetExitCodeThread(SafeThreadHandle handle, UInt32* exitCode);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetThreadPriority(SafeThreadHandle handle, out Int32 priority);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetThreadPriority(SafeThreadHandle handle, Int32 priority);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetThreadPriorityBoost(SafeThreadHandle handle, [MarshalAs(UnmanagedType.Bool)] out bool pDisablePriorityBoost);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetThreadPriorityBoost(SafeThreadHandle handle, [MarshalAs(UnmanagedType.Bool)] bool pDisablePriorityBoost);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetThreadIOPendingFlag(SafeThreadHandle handle, [MarshalAs(UnmanagedType.Bool)] out bool pDisablePriorityBoost);

		[DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetThreadTimes")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetThreadTimesNative(SafeThreadHandle hProcess,
			out System.Runtime.InteropServices.ComTypes.FILETIME lpCreationTime,
			out System.Runtime.InteropServices.ComTypes.FILETIME lpExitTime,
			out System.Runtime.InteropServices.ComTypes.FILETIME lpKernelTime,
			out System.Runtime.InteropServices.ComTypes.FILETIME lpUserTime
		);

		[DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = false)]
		internal static extern unsafe UInt32 GetThreadDescription(SafeThreadHandle handle, Char** exitCode);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern Int32 SuspendThread(SafeThreadHandle handle);
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern Int32 ResumeThread(SafeThreadHandle handle);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool TerminateThread(SafeThreadHandle handle, UInt32 exitCode);

#if x86
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetThreadSelectorEntry(SafeThreadHandle handle, UInt32 selector, out SelectorEntry.Native entry);
#endif
#if x64
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Wow64GetThreadSelectorEntry(SafeThreadHandle handle, UInt32 selector, out SelectorEntry.Native entry);
#endif

		[DllImport("Advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool OpenThreadToken(SafeThreadHandle procHandle, UInt32 access, [MarshalAs(UnmanagedType.Bool)] bool IgnoreImpersonation, out SafeTokenHandle tokenHandle);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "QueueUserAPC2")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool QueueUserAPC2Native(IntPtr pfnAPC, SafeThreadHandle procHandle, IntPtr dwData, QueueAPC2Flags flags);
	}
}
