using Henke37.Win32.BatteryAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatteryTest {
	class Program {
		static void Main(string[] args) {
			var devices = BatteryPort.GetBatteries();
		}
	}
}
