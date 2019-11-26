using Henke37.Win32.Base;
using Henke37.Win32.Base.AccessRights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Henke37.Win32.BatteryAccess {
	public class BatteryPort {
		internal NativeFileObject file;

		internal const UInt64 InvalidTag = 0;

		public BatteryPort(string path, bool readOnly = true) {
			var rights = readOnly ? FileObjectAccessRights.GenericRead : FileObjectAccessRights.GenericRead | FileObjectAccessRights.GenericWrite;
			var share= readOnly ? FileShareMode.Read : FileShareMode.Write | FileShareMode.Read;
			file = NativeFileObject.Open(path, rights, share, FileDisposition.OpenExisting, 0);
		}

		public UInt64 BatteryTag {
			get => GetBatteryTag();
		}

		private UInt64 GetBatteryTag() {
			UInt64 timeout = 0;
			UInt64 tagBuff= InvalidTag;
			file.DeviceControlInputOutput<UInt64, UInt64>(DeviceIoControlCode.BatteryQueryTag, ref timeout, ref tagBuff);

			return tagBuff;
		}
	}
}
