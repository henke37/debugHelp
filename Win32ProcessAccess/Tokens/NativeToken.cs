﻿using Henke37.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace Henke37.Win32.Tokens {
#if NETFRAMEWORK
	[HostProtection(SecurityInfrastructure = true)]
#endif
	public class NativeToken : IDisposable, IEquatable<NativeToken> {
		private SafeTokenHandle tokenHandle;

		internal NativeToken(SafeTokenHandle tokenHandle) {
			this.tokenHandle = tokenHandle;
		}

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[ReliabilityContract(Consistency.MayCorruptProcess, Cer.None)]
		public NativeToken Reopen(TokenAccessLevels rights = TokenAccessLevels.AllAccess, bool inheritable = false) {
			var rawHandle = SafeKernelObjHandle.DuplicateHandleLocal(tokenHandle.DangerousGetHandle(), (uint)rights, inheritable, SafeKernelObjHandle.DuplicateOptions.None);
			return new NativeToken(new SafeTokenHandle(rawHandle));
		}

		public void Dispose() => tokenHandle.Dispose();
		public void Close() => tokenHandle.Close();

		public bool Equals(NativeToken other) {
			var status = NtCompareTokens(tokenHandle, other.tokenHandle, out bool equal);
			if(status.Severity != PInvoke.NTSTATUS.SeverityCode.STATUS_SEVERITY_SUCCESS) {
				throw new PInvoke.NTStatusException(status);
			}
			return equal;
		}

		public TokenElevationType ElevationType {
			get {
				GetTokenInformation(TokenInformationClass.ElevationType, out TokenElevationType elevationType);
				return elevationType;
			}
		}

		[HostProtection(MayLeakOnAbort = true)]
		public NativeToken GetLinkedToken() {
			GetTokenInformation(TokenInformationClass.LinkedToken, out IntPtr newHandle);
			return new NativeToken(new SafeTokenHandle(newHandle));
		}

		internal unsafe void GetTokenInformation<T>(TokenInformationClass infoClass, out T buff) where T : unmanaged {
			fixed (void* buffP = &buff) {
				bool success = GetTokenInformation(tokenHandle, infoClass, buffP, (uint)sizeof(T), out _);
				if(!success) throw new Win32Exception();
			}
		}

		public unsafe void AdjustPrivilege(UInt64 priv, PrivilegeAttributes newState, out PrivilegeAttributes oldState) {
			TokenPrivilege inBuff = new TokenPrivilege(priv, newState);
			void* inBuffP = (void*)&inBuff;
			TokenPrivilege outBuff = new TokenPrivilege();
			void* outBuffP = (void*)&outBuff;

			uint size= (uint)sizeof(TokenPrivilege);

			bool success=AdjustTokenPrivileges(tokenHandle, false, inBuffP, size, outBuffP, out _);
			if(!success) throw new Win32Exception();

			oldState = outBuff.Privilege.Attributes;
		}

		public LuidAndAttributes[] Privileges {
			get {
				return GetPrivileges();
			}
		}

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
		#region Information properties
		public TokenType TokenType {
			get {
				GetTokenInformation(TokenInformationClass.Type, out TokenType type);
				return type;
			}
		}

		public UInt32 SessionId {
			get {
				GetTokenInformation(TokenInformationClass.SessionId, out UInt32 sessionId);
				return sessionId;
			}
		}

		public bool HasRestrictions {
			get {
				GetTokenInformation(TokenInformationClass.HasRestrictions, out UInt32 result);
				return result != 0;
			}
		}

		public bool VirtualizationAllowed {
			get {
				GetTokenInformation(TokenInformationClass.VirtualizationAllowed, out UInt32 result);
				return result != 0;
			}
		}

		public bool VirtualizationEnabled {
			get {
				GetTokenInformation(TokenInformationClass.VirtualizationEnabled, out UInt32 result);
				return result != 0;
			}
		}

		public bool UIAccess {
			get {
				GetTokenInformation(TokenInformationClass.UIAccess, out UInt32 result);
				return result != 0;
			}
		}

		public bool SandBoxInert {
			get {
				GetTokenInformation(TokenInformationClass.SandBoxInert, out UInt32 result);
				return result != 0;
			}
		}

		public bool IsElevated {
			get {
				GetTokenInformation(TokenInformationClass.Elevation, out UInt32 result);
				return result != 0;
			}
		}

		public MandatoryPolicy MandatoryPolicy {
			get {
				GetTokenInformation(TokenInformationClass.MandatoryPolicy, out MandatoryPolicy policy);
				return policy;
			}
		}

		public ImpersonationLevel ImpersonationLevel {
			get {
				GetTokenInformation(TokenInformationClass.ImpersonationLevel, out ImpersonationLevel level);
				return level;
			}
		}

		public bool IsAppContainer {
			get {
				GetTokenInformation(TokenInformationClass.IsAppContainer, out UInt32 result);
				return result != 0;
			}
		}

		public UInt32 AppContainerNumber {
			get {
				GetTokenInformation(TokenInformationClass.AppContainerNumber, out UInt32 result);
				return result;
			}
		}

		public TokenSource TokenSource {
			get {
				TokenSource.Native native;
				GetTokenInformation(TokenInformationClass.Source, out native);
				return new TokenSource(native);
			}
		}

		public TokenStatistics TokenStatistics {
			get {
				TokenStatistics native;
				GetTokenInformation(TokenInformationClass.Statistics, out native);
				return native;
			}
		}

		public UInt64 OriginatingLogonSession {
			get {
				GetTokenInformation(TokenInformationClass.Origin, out UInt64 result);
				return result;
			}
		}
		#endregion

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
	}
}