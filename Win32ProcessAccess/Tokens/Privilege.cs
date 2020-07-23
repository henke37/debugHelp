using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Henke37.Win32.Tokens {
	public static class Privilege {

		public static UInt64 AssignPrimaryToken => LookupPrivilege("SeAssignPrimaryTokenPrivilege");
		public static UInt64 Audit => LookupPrivilege("SeAuditPrivilege");
		public static UInt64 Backup => LookupPrivilege("SeBackupPrivilege");
		public static UInt64 ChangeNotify => LookupPrivilege("SeChangeNotifyPrivilege");
		public static UInt64 CreateGlobal => LookupPrivilege("SeCreateGlobalPrivilege");
		public static UInt64 CreatePagefile => LookupPrivilege("SeCreatePagefilePrivilege");
		public static UInt64 CreatePermantent => LookupPrivilege("SeCreatePermanentPrivilege");
		public static UInt64 CreateSymbolicLink => LookupPrivilege("SeCreateSymbolicLinkPrivilege");
		public static UInt64 CreateToken => LookupPrivilege("SeCreateTokenPrivilege");
		public static UInt64 Debug => LookupPrivilege("SeDebugPrivilege");
		public static UInt64 DelegateSessionUserImpersonate => LookupPrivilege("SeDelegateSessionUserImpersonatePrivilege");
		public static UInt64 EnableDelegation => LookupPrivilege("SeEnableDelegationPrivilege");
		public static UInt64 Impersonate => LookupPrivilege("SeImpersonatePrivilege");
		public static UInt64 IncreaseBasePriority => LookupPrivilege("SeIncreaseBasePriorityPrivilege");
		public static UInt64 IncreaseQuota => LookupPrivilege("SeIncreaseQuotaPrivilege");
		public static UInt64 IncreaseWorkingSet => LookupPrivilege("SeIncreaseWorkingSetPrivilege");
		public static UInt64 LoadDriver => LookupPrivilege("SeLoadDriverPrivilege");
		public static UInt64 LockMemory => LookupPrivilege("SeLockMemoryPrivilege");
		public static UInt64 MachineAccount => LookupPrivilege("SeMachineAccountPrivilege");
		public static UInt64 ManageVolume => LookupPrivilege("SeManageVolumePrivilege");
		public static UInt64 ProfileSingleProcess => LookupPrivilege("SeProfileSingleProcessPrivilege");
		public static UInt64 Relabel => LookupPrivilege("SeRelabelPrivilege");
		public static UInt64 RemoteShutdown => LookupPrivilege("SeRemoteShutdownPrivilege");
		public static UInt64 Restore => LookupPrivilege("SeRestorePrivilege");
		public static UInt64 Security => LookupPrivilege("SeSecurityPrivilege");
		public static UInt64 Shutdown => LookupPrivilege("SeShutdownPrivilege");
		public static UInt64 SyncAgent => LookupPrivilege("SeSyncAgentPrivilege");
		public static UInt64 SystemEnvironment => LookupPrivilege("SeSystemEnvironmentPrivilege");
		public static UInt64 SystemProfile => LookupPrivilege("SeSystemProfilePrivilege");
		public static UInt64 SystemTime => LookupPrivilege("SeSystemtimePrivilege");
		public static UInt64 TakeOwnership => LookupPrivilege("SeTakeOwnershipPrivilege");
		public static UInt64 TCB => LookupPrivilege("SeTcbPrivilege");
		public static UInt64 TimeZone => LookupPrivilege("SeTimeZonePrivilege");
		public static UInt64 TrustedCredman => LookupPrivilege("SeTrustedCredManAccessPrivilege");
		public static UInt64 Undock => LookupPrivilege("SeUndockPrivilege");
		public static UInt64 UnsolicitedInput => LookupPrivilege("SeUnsolicitedInputPrivilege");

		internal static UInt64 LookupPrivilege(string name) {
			bool success = LookupPrivilegeValueW(null, name, out UInt64 luid);
			if(!success) throw new Win32Exception();
			return luid;
		}

		internal static string LookupPrivilegeName(UInt64 luid) {
			UInt32 size = 45;
			char[] nameBuff = new char[size];
			bool success = LookupPrivilegeNameW(null, ref luid, nameBuff, ref size);
			if(!success) throw new Win32Exception();
			return new String(nameBuff, 0, (int)size);
		}

		[DllImport("Advapi32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool LookupPrivilegeValueW([MarshalAs(UnmanagedType.LPWStr)] string? system, string name, out UInt64 luid);

		[DllImport("Advapi32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool LookupPrivilegeNameW([MarshalAs(UnmanagedType.LPWStr)] string? system, ref UInt64 luid, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] char[] name, ref UInt32 nameLen);
	}

	[Flags]
	public enum PrivilegeAttributes : UInt32 {
		None = 0,
		EnabledByDefault = 0x00000001,
		Enabled = 0x00000002,
		Removed = 0X00000004
	}

	[StructLayout(LayoutKind.Sequential, Pack=1, Size = 12)]
	public struct LuidAndAttributes {
		[DebuggerDisplay("{PrivilegeName} {Attributes}")]
		public UInt64 LUID;
		public PrivilegeAttributes Attributes;

		public LuidAndAttributes(UInt64 LUID, PrivilegeAttributes attributes) {
			this.LUID = LUID;
			Attributes = attributes;
		}

		public string PrivilegeName => Privilege.LookupPrivilegeName(LUID);
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct TokenPrivilege {
		private UInt32 Count;

		public LuidAndAttributes Privilege;

		public TokenPrivilege(UInt64 LUID, PrivilegeAttributes attributes) : this() {
			Count = 1;
			Privilege = new LuidAndAttributes(LUID, attributes);
		}
	}
}
