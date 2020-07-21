using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Henke37.Win32.Base.SafeHandles {
	internal abstract class SafeKernelObjHandle : SafeHandleZeroOrMinusOneIsInvalid {
		private const uint FlagInherit = 0x00000001;
		private const uint FlagProtectFromClose = 0x00000002;

		protected static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

		internal SafeKernelObjHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle) {
			InitialSetHandle(handle);
		}
		protected SafeKernelObjHandle(bool ownsHandle) : base(ownsHandle) {
		}

		internal void InitialSetHandle(IntPtr h) {
			Debug.Assert(base.IsInvalid, "Safe handle should only be set once");
			base.SetHandle(h);
		}

		[SuppressUnmanagedCodeSecurity]
		[SecuritySafeCritical]
		override protected bool ReleaseHandle() {
			return CloseHandle(handle);
		}

		public bool Inheritable {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var success = GetHandleInformation(handle, out UInt32 flags);
				if(!success) throw new Win32Exception();
				return (flags & FlagInherit) != 0;
			}
			[SecurityCritical]
			set {
				var success = SetHandleInformation(handle, FlagInherit, value ? FlagInherit : 0);
				if(!success) throw new Win32Exception();
			}
		}

		public bool ProtectedFromClose {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var success = GetHandleInformation(handle, out UInt32 flags);
				if(!success) throw new Win32Exception();
				return (flags & FlagProtectFromClose) != 0;
			}
			[SecurityCritical]
			set {
				var success = SetHandleInformation(handle, FlagProtectFromClose, value ? FlagProtectFromClose : 0);
				if(!success) throw new Win32Exception();
			}
		}

		public string ObjectTypeName {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var info = new PublicObjectTypeInformation();
				QueryObjectInformation(ObjectInformationClass.ObjectTypeInformation, ref info);

				return info.TypeName;
			}
		}

		public PublicObjectBasicInformation ObjectInformation {
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get {
				var info = new PublicObjectBasicInformation.Native();
				QueryObjectInformation(ObjectInformationClass.ObjectBasicInformation, ref info);

				return info.AsManaged();
			}
		}

		internal unsafe T QueryObjectInformation<T>(ObjectInformationClass infoClass, ref T buff) where T : unmanaged {
			fixed(void* buffP = &buff) {
				PInvoke.NTSTATUS status = NtQueryObject(handle, infoClass, buffP, (uint)sizeof(T), out _);
				if(status.Severity != PInvoke.NTSTATUS.SeverityCode.STATUS_SEVERITY_SUCCESS) throw new PInvoke.NTStatusException(status);
				return buff;
			}
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.MayCorruptProcess, Cer.None)]
		internal static unsafe IntPtr DuplicateHandleLocal(IntPtr sourceHandle, uint desiredAccess, bool inheritHandle, DuplicateOptions options) {
			IntPtr newHandle = IntPtr.Zero;
			bool success = DuplicateHandle(SafeProcessHandle.CurrentProcess, sourceHandle, SafeProcessHandle.CurrentProcess, (IntPtr)(int)&newHandle, desiredAccess, inheritHandle, options);

			if(!success) throw new Win32Exception();
			return newHandle;
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CompareObjectHandles(IntPtr handle1, IntPtr handle2);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetHandleInformation(IntPtr handle, out UInt32 flags);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetHandleInformation(IntPtr handle, UInt32 mask, UInt32 flags);

		[Flags]
		public enum DuplicateOptions : uint {
			None = 0,
			CloseSource = 0x00000001,// Closes the source handle. This occurs regardless of any error status returned.
			SameAccess = 0x00000002, //Ignores the dwDesiredAccess parameter. The duplicate handle has the same access as the source handle.
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DuplicateHandle(SafeProcessHandle sourceProcess, IntPtr sourceHandle, SafeProcessHandle destinationProcess, IntPtr destinationHandlePtr, uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, DuplicateOptions dwOptions);

		[DllImport("Ntdll.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern unsafe PInvoke.NTSTATUS NtQueryObject(IntPtr handle, ObjectInformationClass informationClass, void* buffer, uint bufferLength, out uint returnLength);
	}
}
