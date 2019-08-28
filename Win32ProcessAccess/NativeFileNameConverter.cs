﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.DebugHelp.Win32 {
	public class NativeFileNameConverter {

		private Dictionary<string, string> deviceMap;

		public NativeFileNameConverter() {
			deviceMap = BuildDeviceMap();
		}

		private Dictionary<string, string> BuildDeviceMap() {
			var map = new Dictionary<string, string>(27);

			foreach(var drive in Environment.GetLogicalDrives()) {
				string cleanDrive = drive.Trim('\\');
				map.Add(cleanDrive, DosDeviceToNative(cleanDrive));
			}

			return map;
		}

		public string NativeNameToDosName(string nativeName) {
			foreach(var kv in deviceMap) {
				if(nativeName.StartsWith(kv.Value)) {
					return nativeName.Replace(kv.Value, kv.Key);
				}
			}
			throw new Exception();
		}

		public static string DosDeviceToNative(string deviceName) {
			var sb = new StringBuilder(300);
			var result = QueryDosDeviceW(deviceName, sb, (uint)sb.Capacity);
			if(result == 0) throw new Win32Exception();
			return sb.ToString();
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern UInt32 QueryDosDeviceW([MarshalAs(UnmanagedType.LPWStr)] string deviceName, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder targetPath, UInt32 targetPathBufferLen);
	}
}
