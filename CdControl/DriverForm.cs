using Henke37.Win32.CdAccess;
using Henke37.Win32.DeviceEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CdControl {
	public partial class DriverForm : Form {

		private CdDrive cdDrive;

		private List<DeviceInterface> drives;

		public DriverForm() {
			InitializeComponent();
			drives = CdDrive.GetCdDrives().ToList();
			cdDrive = new CdDrive(@"\\.\\e:");
		}

		private void Eject_btn_Click(object sender, EventArgs e) {
			cdDrive.Eject();
		}

		private void Load_btn_Click(object sender, EventArgs e) {
			cdDrive.Load();
		}

		private void GetTOC_btn_Click(object sender, EventArgs e) {
			var toc=cdDrive.GetTOC();
		}
	}
}
