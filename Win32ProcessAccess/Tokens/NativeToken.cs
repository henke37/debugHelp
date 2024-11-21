using Henke37.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;

namespace Henke37.Win32.Tokens {
#if NETFRAMEWORK
	[HostProtection(SecurityInfrastructure = true)]
#endif
	[SuppressUnmanagedCodeSecurity]
	public class NativeToken : IDisposable, IEquatable<NativeToken> {
		private SafeTokenHandle tokenHandle;

		private const int ERROR_NOT_ALL_ASSIGNED = 1300;

		internal NativeToken(SafeTokenHandle tokenHandle) {
			this.tokenHandle = tokenHandle;
		}

#if NETFRAMEWORK
		[HostProtection(MayLeakOnAbort = true)]
#endif
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[ReliabilityContract(Consistency.MayCorruptProcess, Cer.None)]
        [SecuritySafeCritical]
        public NativeToken Reopen(TokenAccessLevels rights = TokenAccessLevels.AllAccess, bool inheritable = false) {
			var rawHandle = SafeKernelObjHandle.DuplicateHandleLocal(tokenHandle.DangerousGetHandle(), (uint)rights, inheritable, SafeKernelObjHandle.DuplicateOptions.None);
			return new NativeToken(new SafeTokenHandle(rawHandle));
		}

		public void Dispose() => tokenHandle.Dispose();
		public void Close() => tokenHandle.Close();

        [SecuritySafeCritical]
        public bool Equals(NativeToken other) {
			var status = NtCompareTokens(tokenHandle, other.tokenHandle, out bool equal);
			if(status.Severity != PInvoke.NTSTATUS.SeverityCode.STATUS_SEVERITY_SUCCESS) {
				throw new PInvoke.NTStatusException(status);
			}
			return equal;
		}

        [SecurityCritical]
        internal unsafe void GetTokenInformation<T>(TokenInformationClass infoClass, out T buff) where T : unmanaged {
			fixed (void* buffP = &buff) {
				bool success = GetTokenInformation(tokenHandle, infoClass, buffP, (uint)sizeof(T), out _);
				if(!success) throw new Win32Exception();
			}
		}

		[SecuritySafeCritical]
		internal unsafe byte[] GetTokenInformation(TokenInformationClass infoClass) {
			uint retLen=0;

            try {
				bool success = GetTokenInformation(tokenHandle, infoClass, null, 0, out retLen);
				if (!success) throw new Win32Exception();
			} catch(Win32Exception err) when(err.NativeErrorCode==122) {}

			byte[] buff = new byte[retLen];

			fixed (byte* buffP = buff) {
				bool success = GetTokenInformation(tokenHandle, infoClass, buffP, retLen, out retLen);
				if (!success) throw new Win32Exception();
			}

			return buff;
        }

        [SecuritySafeCritical]
        public unsafe void AdjustPrivilege(UInt64 priv, PrivilegeAttributes newState, out PrivilegeAttributes oldState) {
			TokenPrivilege inBuff = new TokenPrivilege(priv, newState);
			void* inBuffP = (void*)&inBuff;
			TokenPrivilege outBuff = new TokenPrivilege();
			void* outBuffP = (void*)&outBuff;

			uint size = (uint)sizeof(TokenPrivilege);

			bool success = AdjustTokenPrivileges(tokenHandle, false, inBuffP, size, outBuffP, out _);
			if(!success) throw new Win32Exception();
			int err = Marshal.GetLastWin32Error();

			if(err == ERROR_NOT_ALL_ASSIGNED) {
				throw new Win32Exception(err);
			}

			oldState = outBuff.Privilege.Attributes;
		}

        [SecuritySafeCritical]
        private unsafe LuidAndAttributes[] GetPrivileges() {
			UInt32 size = (UInt32)(4 + sizeof(LuidAndAttributes) * 36);
			byte[] buff = new byte[size];
			buff[0] = 36;
			fixed(byte* buffP = buff) {
				bool success = GetTokenInformation(tokenHandle, TokenInformationClass.Privileges, buffP, size, out _);
				if(!success) throw new Win32Exception();
				int privCount = buff[0];

				var outBuff = new LuidAndAttributes[privCount];

				var arrStart = (LuidAndAttributes*)(buffP + 4);

				for(int privIndex = 0; privIndex < privCount; privIndex++) {

					outBuff[privIndex] = arrStart[privIndex];
				}

				return outBuff;
			}
		}


        [SecurityCritical]
        private unsafe SecurityIdentifier GetSingleSIDAndAtts(TokenInformationClass infoClass) {
            byte[] buff = GetTokenInformation(infoClass);

            fixed (byte* buffP = buff) {
                var sidAtts = (SID_AND_ATTRIBUTES*)buffP;
                return new SecurityIdentifier(sidAtts->pSid);
            }
        }

        [SecurityCritical]
        private unsafe SecurityIdentifier GetSIDInfo(TokenInformationClass infoClass) {
            byte[] buff = GetTokenInformation(infoClass);

            fixed (byte* buffP = buff) {
                return new SecurityIdentifier(*(IntPtr*)buffP);
            }
        }

        [SecurityCritical]
        private unsafe GroupEntry[] GetGroups(TokenInformationClass infoClass) {
            byte[] buff = GetTokenInformation(infoClass);

            fixed (byte* buffP = buff) {
                uint entryC = *(UInt32*)buffP;

                var entryP = (SID_AND_ATTRIBUTES*)(buffP + sizeof(UInt32));
                var entries = new GroupEntry[entryC];
                for (var entryI = 0; entryI < entryC; entryI++, entryP++) {
                    entries[entryI] = new GroupEntry() {
                        SID = new SecurityIdentifier(entryP->pSid),
                        Flags = (GroupAttributeFlags)entryP->dwAttributes
                    };
                }

                return entries;
            }
        }

        #region Token Information

        public SecurityIdentifier User {
            [SecuritySafeCritical]
            get {
				return GetSingleSIDAndAtts(TokenInformationClass.User);
			}
		}

		public GroupEntry[] Groups {
            [SecuritySafeCritical]
            get {
				return GetGroups(TokenInformationClass.Groups);
			}
		}

		public SecurityIdentifier Owner {
            [SecuritySafeCritical]
            get {
				return GetSIDInfo(TokenInformationClass.Owner);
			}
		}

		public SecurityIdentifier PrimaryGroup {
            [SecuritySafeCritical]
            get {
				return GetSIDInfo(TokenInformationClass.PrimaryGroup);
			}
		}

		public SecurityIdentifier IntegrityLevel {
            [SecuritySafeCritical]
            get {
				return GetSingleSIDAndAtts(TokenInformationClass.IntegrityLevel);
			}
		}

		public GroupEntry[] Capabilities {
            [SecuritySafeCritical]
            get {
				return GetGroups(TokenInformationClass.Capabilities);
			}
		}

		public GroupEntry[] DeviceGroups {
            [SecuritySafeCritical]
            get {
				return GetGroups(TokenInformationClass.DeviceGroups);
			}
		}

        public TokenElevationType ElevationType {
            [SecuritySafeCritical]
            get {
                GetTokenInformation(TokenInformationClass.ElevationType, out TokenElevationType elevationType);
                return elevationType;
            }
        }

        public LuidAndAttributes[] Privileges {
            get {
                return GetPrivileges();
            }
        }

#if NETFRAMEWORK
        [HostProtection(MayLeakOnAbort = true)]
#endif
        [SecuritySafeCritical]
        public NativeToken GetLinkedToken() {
            GetTokenInformation(TokenInformationClass.LinkedToken, out IntPtr newHandle);
            return new NativeToken(new SafeTokenHandle(newHandle));
        }

        public TokenType TokenType {
            [SecuritySafeCritical]
            get {
				GetTokenInformation(TokenInformationClass.Type, out TokenType type);
				return type;
			}
		}

		public UInt32 SessionId {
            [SecuritySafeCritical]
            get {
				GetTokenInformation(TokenInformationClass.SessionId, out UInt32 sessionId);
				return sessionId;
			}
		}

		public bool HasRestrictions {
            [SecuritySafeCritical]
            get {
				GetTokenInformation(TokenInformationClass.HasRestrictions, out UInt32 result);
				return result != 0;
			}
        }

        public bool VirtualizationAllowed {
            [SecuritySafeCritical]
            get {
				GetTokenInformation(TokenInformationClass.VirtualizationAllowed, out UInt32 result);
				return result != 0;
			}
		}

		public bool VirtualizationEnabled {
            [SecuritySafeCritical]
            get {
				GetTokenInformation(TokenInformationClass.VirtualizationEnabled, out UInt32 result);
				return result != 0;
			}
		}

		public bool UIAccess {
            [SecuritySafeCritical]
            get {
				GetTokenInformation(TokenInformationClass.UIAccess, out UInt32 result);
				return result != 0;
			}
		}

		public bool SandBoxInert {
            [SecuritySafeCritical]
            get {
				GetTokenInformation(TokenInformationClass.SandBoxInert, out UInt32 result);
				return result != 0;
			}
		}

		public bool IsElevated {
            [SecuritySafeCritical]
            get {
				GetTokenInformation(TokenInformationClass.Elevation, out UInt32 result);
				return result != 0;
			}
		}

		public MandatoryPolicy MandatoryPolicy {
            [SecuritySafeCritical]
            get {
				GetTokenInformation(TokenInformationClass.MandatoryPolicy, out MandatoryPolicy policy);
				return policy;
			}
		}

		public ImpersonationLevel ImpersonationLevel {
            [SecuritySafeCritical]
            get {
				GetTokenInformation(TokenInformationClass.ImpersonationLevel, out ImpersonationLevel level);
				return level;
			}
		}

		public bool IsAppContainer {
            [SecuritySafeCritical]
            get {
				GetTokenInformation(TokenInformationClass.IsAppContainer, out UInt32 result);
				return result != 0;
			}
		}

		public UInt32 AppContainerNumber {
            [SecuritySafeCritical]
            get {
				GetTokenInformation(TokenInformationClass.AppContainerNumber, out UInt32 result);
				return result;
			}
		}

		public TokenSource TokenSource {
            [SecuritySafeCritical]
            get {
				TokenSource.Native native;
				GetTokenInformation(TokenInformationClass.Source, out native);
				return new TokenSource(native);
			}
		}

		public TokenStatistics TokenStatistics {
            [SecuritySafeCritical]
            get {
				TokenStatistics native;
				GetTokenInformation(TokenInformationClass.Statistics, out native);
				return native;
			}
		}

		public UInt64 OriginatingLogonSession {
            [SecuritySafeCritical]
            get {
				GetTokenInformation(TokenInformationClass.Origin, out UInt64 result);
				return result;
			}
		}
        #endregion

        [SecuritySafeCritical]
        public NativeToken Duplicate(ImpersonationLevel impersonation) {
			var success = DuplicateToken(tokenHandle, impersonation, out var newToken);
			if(!success) throw new Win32Exception();
			return new NativeToken(newToken);
		}

		[DllImport("Advapi32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DuplicateToken(SafeTokenHandle currentToken, ImpersonationLevel impersonation, out SafeTokenHandle newToken);

		[DllImport("Ntdll.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern unsafe PInvoke.NTSTATUS NtCompareTokens(SafeTokenHandle handle1, SafeTokenHandle handle2, [MarshalAs(UnmanagedType.Bool)] out bool equal);

		[DllImport("Advapi32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool GetTokenInformation(SafeTokenHandle handle, TokenInformationClass informationClass, void* outBuff, UInt32 outBuffLen, out UInt32 retLen);

		[DllImport("Advapi32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool AdjustTokenPrivileges(SafeTokenHandle handle, [MarshalAs(UnmanagedType.Bool)] bool disableEverything, void* newState, UInt32 outBuffLen, void* oldState, out UInt32 retLen);

		[DllImport("Advapi32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool AdjustTokenGroups(SafeTokenHandle handle, [MarshalAs(UnmanagedType.Bool)] bool resetToDefault, void* newState, UInt32 outBuffLen, void* oldState, out UInt32 retLen);
		
		[DllImport("Ntdll.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern unsafe PInvoke.NTSTATUS NtCreateLowBoxToken(
  out SafeTokenHandle NewTokenHandle,
  SafeTokenHandle ExistingTokenHandle,
  UInt32 DesiredAccess,
  //_in_ POBJECT_ATTRIBUTES  ObjectAttributes,
  IntPtr ObjectAttributes,
  //_in_ PSID                PackageSid,
  IntPtr PackageSid,
  UInt32 CapabilityCount,
  SID_AND_ATTRIBUTES* Capabilities,
  UInt32 HandleCount,
  //_in_ HANDLE*             Handles
  IntPtr Handles
);

	}
}