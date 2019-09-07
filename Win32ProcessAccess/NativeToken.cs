using Henke37.DebugHelp.Win32.SafeHandles;
using System;
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
			if(status.Severity!=PInvoke.NTSTATUS.SeverityCode.STATUS_SEVERITY_SUCCESS) {
				throw new PInvoke.NTStatusException(status);
			}
			return equal;
		}

		[DllImport("Ntdll.dll", ExactSpelling = true, SetLastError = false)]
		internal static extern unsafe PInvoke.NTSTATUS NtCompareTokens(SafeTokenHandle handle1, SafeTokenHandle handle2, [MarshalAs(UnmanagedType.Bool)] out bool equal);
	}
}