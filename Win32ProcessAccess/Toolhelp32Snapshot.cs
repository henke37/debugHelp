using Henke37.DebugHelp.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Henke37.DebugHelp.Win32 {
	public class Toolhelp32Snapshot : IDisposable {
		internal SafeToolhelp32SnapshotHandle handle;
		private const int ErrNoMoreFiles = 18;

		public Toolhelp32Snapshot(Toolhelp32SnapshotFlags flags) {
			handle = CreateToolhelp32Snapshot((uint)flags, 0);
			if(handle.IsInvalid) throw new Win32Exception();
		}
		public Toolhelp32Snapshot(Toolhelp32SnapshotFlags flags, UInt32 processId) {
			if(processId == 0) throw new ArgumentOutOfRangeException(nameof(processId), "The process id can't be zero!");
			if((flags & Toolhelp32SnapshotFlags.Thread) == Toolhelp32SnapshotFlags.Thread) throw new ArgumentException("Per process thread filtering doesn't work.");
			handle = CreateToolhelp32Snapshot((uint)flags, processId);
			if(handle.IsInvalid) throw new Win32Exception();
		}

		public IEnumerable<ModuleEntry> GetModules() {
			ModuleEntry.Native native = new ModuleEntry.Native();
			native.dwSize = (uint)Marshal.SizeOf<ModuleEntry.Native>();
			try {
				bool success = Module32FirstW(handle, ref native);
				if(!success) throw new Win32Exception();
			} catch(Win32Exception err) when(err.NativeErrorCode == ErrNoMoreFiles) {
				yield break;
			}

			yield return native.AsManaged();

			for(; ; ) {
				try {
					bool success = Module32NextW(handle, ref native);
					if(!success) throw new Win32Exception();
				} catch(Win32Exception err) when(err.NativeErrorCode == ErrNoMoreFiles) {
					yield break;
				}

				yield return native.AsManaged();
			}
		}

		public IEnumerable<ProcessEntry> GetProcesses() {
			ProcessEntry.Native native = new ProcessEntry.Native();
			native.dwSize = (uint)Marshal.SizeOf<ProcessEntry.Native>();
			try {
				bool success = Process32FirstW(handle, ref native);
				if(!success) throw new Win32Exception();
			} catch(Win32Exception err) when(err.NativeErrorCode == ErrNoMoreFiles) {
				yield break;
			}

			yield return native.AsManaged();

			for(; ; ) {
				try {
					bool success = Process32NextW(handle, ref native);
					if(!success) throw new Win32Exception();
				} catch(Win32Exception err) when(err.NativeErrorCode == ErrNoMoreFiles) {
					yield break;
				}

				yield return native.AsManaged();
			}
		}

		public IEnumerable<ThreadEntry> GetThreads() {
			ThreadEntry.Native native = new ThreadEntry.Native();
			native.dwSize = (uint)Marshal.SizeOf<ThreadEntry.Native>();
			try {
				bool success = Thread32First(handle, ref native);
				if(!success) throw new Win32Exception();
			} catch(Win32Exception err) when(err.NativeErrorCode == ErrNoMoreFiles) {
				yield break;
			}

			yield return native.AsManaged();

			for(; ; ) {
				try {
					bool success = Thread32Next(handle, ref native);
					if(!success) throw new Win32Exception();
				} catch(Win32Exception err) when(err.NativeErrorCode == ErrNoMoreFiles) {
					yield break;
				}

				yield return native.AsManaged();
			}
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern SafeToolhelp32SnapshotHandle CreateToolhelp32Snapshot(UInt32 flags, UInt32 procId);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Module32FirstW(SafeToolhelp32SnapshotHandle handle, ref ModuleEntry.Native moduleEntry);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Module32NextW(SafeToolhelp32SnapshotHandle handle, ref ModuleEntry.Native moduleEntry);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Process32FirstW(SafeToolhelp32SnapshotHandle handle, ref ProcessEntry.Native moduleEntry);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Process32NextW(SafeToolhelp32SnapshotHandle handle, ref ProcessEntry.Native moduleEntry);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Thread32First(SafeToolhelp32SnapshotHandle handle, ref ThreadEntry.Native moduleEntry);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool Thread32Next(SafeToolhelp32SnapshotHandle handle, ref ThreadEntry.Native moduleEntry);

		public void Dispose() {
			((IDisposable)handle).Dispose();
		}
	}
}
