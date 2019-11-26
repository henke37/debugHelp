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

			foreach(var device in devices) {
				Console.WriteLine(device.FilePath);
				var bat = new BatteryPort(device);
				var tag = bat.BatteryTag;

				var status=bat.GetStatus(tag);
				Console.WriteLine(
					"{0} {1} {2} {3}",
					status.PowerState, status.Voltage,
					status.Rate, status.Capacity
				);
			}
		}
	}
}
