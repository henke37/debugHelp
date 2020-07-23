using Henke37.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Henke37.Win32.Tokens {
#if NETFRAMEWORK
	[HostProtection(SecurityInfrastructure = true)]
#endif
	public class NativeToken : IDisposable, IEquatable<NativeToken> {
		private SafeTokenHandle tokenHandle;

		internal NativeToken(SafeTokenHandle tokenHandle) {
			this.tokenHandle = tokenHandle;
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
				GetTokenInformation(TokenInformationClass.HasRestrictions, out UInt32 restrictions);
				return restrictions != 0;
			}
		}

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