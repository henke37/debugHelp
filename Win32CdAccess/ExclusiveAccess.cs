using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

using Henke37.Win32.AccessRights;
using Henke37.Win32.DeviceEnum;
using Henke37.Win32.Files;
using static System.Collections.Specialized.BitVector32;
using static Henke37.Win32.CdAccess.Configuration;

namespace Henke37.Win32.CdAccess {

	internal enum ExclusiveAccessRequestType {
		QueryState,
		LockDevice,
		UnlockDevice
	}

	[Flags]
	internal enum ExclusiveAccessRequestFlags : UInt32 {
		IgnoreVolume = 1 << 0,
		NoMediaNotifications = 1 << 1
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct ExcusiveAccessRequest {
		public ExclusiveAccessRequestType RequestType;
		public ExclusiveAccessRequestFlags Flags;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal unsafe struct ExclusiveAccessLockRequest {
		public ExcusiveAccessRequest Access;
		public fixed byte CallerName[64];
	}

	public class CdLockToken : IDisposable {
		private bool disposedValue;

		public bool SuppressMediaNotifications;

		private CdDrive cdDrive;

		public CdLockToken(CdDrive cdDrive) {
			this.cdDrive = cdDrive;
		}

		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					// TODO: dispose managed state (managed objects)
				}

				cdDrive.Unlock(SuppressMediaNotifications);
				disposedValue = true;
			}
		}

		
		~CdLockToken()
		{
		    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		    Dispose(disposing: false);
		}

		public void Dispose() {
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}