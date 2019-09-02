using Henke37.Win32.CdAccess;
using System;
using System.Windows.Forms;

namespace CdControl {
	public partial class DriverForm : Form {

		private CdDrive cdDrive;

		public DriverForm() {
			InitializeComponent();
			cdDrive = new CdDrive(@"\\.\\e:");
		}

		private void Eject_btn_Click(object sender, EventArgs e) {
			cdDrive.Eject();
		}

		private void Load_btn_Click(object sender, EventArgs e) {
			cdDrive.Load();
		}
	}
}
