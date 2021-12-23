using System;

namespace Henke37.Win32.Debug.Info {
	internal struct ExceptionDebugInfo {
		internal NativeExceptionRecord exception;
		internal UInt32 firstChance;
	}

	internal unsafe struct NativeExceptionRecord {
		UInt32 ExceptionCode;
		UInt32 ExceptionFlags;
		NativeExceptionRecord* other;
		IntPtr ExceptionAddress;
		UInt32 NumberParameters;
		fixed uint ExceptionInformation[EXCEPTION_MAXIMUM_PARAMETERS];

		const int EXCEPTION_MAXIMUM_PARAMETERS = 15;

		public ExceptionRecord AsManaged() {
			var record = new ExceptionRecord() {
				ExceptionCode = (ExceptionCode)ExceptionCode,
				ExceptionFlags = (ExceptionFlag)ExceptionFlags,
				ExceptionAddress = ExceptionAddress
			};

			if(other!=null) {
				record.Other = other->AsManaged();
			}

			if(NumberParameters>0) {
				record.ExceptionInfo = new uint[NumberParameters];
				for(int i = 0;i<NumberParameters;++i) {
					record.ExceptionInfo[i] = ExceptionInformation[i];
				}
			}

			return record;
		}
	}
}