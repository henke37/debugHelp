using System;
using System.ComponentModel;
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

		[DllImport("Advapi32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool LookupPrivilegeValueW(string? system, string name, out UInt64 luid);
	}

	public enum PrivilegeStatus {
		None = 0,
		EnabledByDefault = 0x00000001,
		Enabled = 0x00000002,
		Removed = 0X00000004
	}
}
