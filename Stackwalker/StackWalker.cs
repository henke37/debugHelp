﻿using DIA;
using Henke37.DebugHelp;
using Henke37.DebugHelp.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stackwalker {
	public class StackWalker {

		private DiaStackWalker walker;
		private StackWalkHelper helper;

		public StackWalker(NativeThread thread, ProcessMemoryAccessor memoryReader) {
			walker = new DiaStackWalker();
			helper = new StackWalkHelper(thread, memoryReader);
		}

		public IEnumerable<IDiaStackFrame> Walk() {
			helper.InitializeForWalk();
			var frames=walker.getEnumFrames(helper);

			IDiaStackFrame[] arr = new IDiaStackFrame[1];
			for(; ; ) {
				frames.Next(1, arr, out _);
				yield return arr[0];
			}
		}
	}
}
