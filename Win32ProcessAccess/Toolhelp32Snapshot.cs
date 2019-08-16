using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
	public class Toolhelp32Snapshot {
		internal SafeToolhelp32SnapshotHandle handle;
		private const int ErrNoMoreFiles=18;

		public Toolhelp32Snapshot(Toolhelp32SnapshotFlags flags) {
		}
		public Toolhelp32Snapshot(Toolhelp32SnapshotFlags flags, UInt32 processId) {
		}

		public IEnumerator<ModuleEntry> GetModules() {
			ModuleEntry.Native native=new ModuleEntry.Native();
			native.dwSize = (uint)Marshal.SizeOf<ModuleEntry.Native>();
			try {
				Module32FirstW(handle, ref native);
			} catch(Win32Exception err ) when(err.NativeErrorCode == ErrNoMoreFiles) {
				yield break;
			}

			yield return native.AsManaged();

			for(; ;) {
				try {
					Module32NextW(handle, ref native);
				} catch(Win32Exception err) when(err.NativeErrorCode == ErrNoMoreFiles) {
					yield break;
				}

				yield return native.AsManaged();
			}
		}

		public IEnumerator<ProcessEntry> GetProcesses() {
			ProcessEntry.Native native = new ProcessEntry.Native();
			native.dwSize = (uint)Marshal.SizeOf<ProcessEntry.Native>();
			try {
				Process32FirstW(handle, ref native);
			} catch(Win32Exception err) when(err.NativeErrorCode == ErrNoMoreFiles) {
				yield break;
			}

			yield return native.AsManaged();

			for(; ; ) {
				try {
					Process32NextW(handle, ref native);
				} catch(Win32Exception err) when(err.NativeErrorCode == ErrNoMoreFiles) {
					yield break;
				}

				yield return native.AsManaged();
			}
		}

		public IEnumerator<ThreadEntry> GetThreads() {
			ThreadEntry.Native native = new ThreadEntry.Native();
			native.dwSize = (uint)Marshal.SizeOf<ThreadEntry.Native>();
			try {
				Thread32First(handle, ref native);
			} catch(Win32Exception err) when(err.NativeErrorCode == ErrNoMoreFiles) {
				yield break;
			}

			yield return native.AsManaged();

			for(; ; ) {
				try {
					Thread32Next(handle, ref native);
				} catch(Win32Exception err) when(err.NativeErrorCode == ErrNoMoreFiles) {
					yield break;
				}

				yield return native.AsManaged();
			}
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Module32FirstW(SafeToolhelp32SnapshotHandle handle, ref ModuleEntry.Native moduleEntry);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Module32NextW(SafeToolhelp32SnapshotHandle handle, ref ModuleEntry.Native moduleEntry);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Process32FirstW(SafeToolhelp32SnapshotHandle handle, ref ProcessEntry.Native moduleEntry);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Process32NextW(SafeToolhelp32SnapshotHandle handle, ref ProcessEntry.Native moduleEntry);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Thread32First(SafeToolhelp32SnapshotHandle handle, ref ThreadEntry.Native moduleEntry);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Thread32Next(SafeToolhelp32SnapshotHandle handle, ref ThreadEntry.Native moduleEntry);

	}
}
