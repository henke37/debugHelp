using Dia2Lib;
using Henke37.DebugHelp.Win32;
using Stackwalker.Stackwalker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stackwalker {
	public class StackWalker {

		private DiaStackWalker walker;
		private StackWalkHelper helper;
		private IDiaStackWalkHelper adapter;

		public StackWalker(NativeThread thread) {
			walker = new DiaStackWalker();
			helper = new StackWalkHelper(thread);
			adapter = new HelperAdapter(helper);
		}

		public void Walk() {
			//walker.getEnumFrames()
		}
	}
}
