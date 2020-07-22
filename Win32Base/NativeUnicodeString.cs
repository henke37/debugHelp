﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Henke37.Win32 {
    [StructLayout(LayoutKind.Sequential)]
    internal struct UNICODE_STRING : IDisposable {
        public ushort Length;
        public ushort MaximumLength;
        private IntPtr buffer;

        public UNICODE_STRING(string s) {
            Length = (ushort)(s.Length * 2);
            MaximumLength = (ushort)(Length + 2);
            buffer = Marshal.StringToHGlobalUni(s);
        }

        public void Dispose() {
            Marshal.FreeHGlobal(buffer);
            buffer = IntPtr.Zero;
        }

        public override string ToString() {
            return Marshal.PtrToStringUni(buffer);
        }
    }
}
