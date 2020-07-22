using Henke37.Win32.SafeHandles;
using System;
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

		[DllImport("Ntdll.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern unsafe PInvoke.NTSTATUS NtCompareTokens(SafeTokenHandle handle1, SafeTokenHandle handle2, [MarshalAs(UnmanagedType.Bool)] out bool equal);

		[DllImport("Advapi32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool GetTokenInformation(SafeTokenHandle handle, TokenInformationClass informationClass, void* outBuff, UInt32 outBuffLen, out UInt32 retLen);
	}
}