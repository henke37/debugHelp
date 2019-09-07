﻿using Henke37.DebugHelp.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
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
				TokenElevationType elevationType=new TokenElevationType();
				GetTokenInformation<TokenElevationType>(TokenInformationClass.TokenElevationType, ref elevationType);
				return elevationType;
			}
		}

		internal unsafe T GetTokenInformation<T>(TokenInformationClass infoClass, ref T buff) where T : unmanaged {
			fixed (void* buffP = &buff) {
				bool success = GetTokenInformation(tokenHandle, infoClass, buffP, (uint)sizeof(T), out _);
				if(!success) throw new Win32Exception();
				return buff;
			}
		}

		[DllImport("Ntdll.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern unsafe PInvoke.NTSTATUS NtCompareTokens(SafeTokenHandle handle1, SafeTokenHandle handle2, [MarshalAs(UnmanagedType.Bool)] out bool equal);

		[DllImport("Ntdll.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern unsafe bool GetTokenInformation(SafeTokenHandle handle, TokenInformationClass informationClass, void* outBuff, UInt32 outBuffLen, out UInt32 retLen);
	}
}