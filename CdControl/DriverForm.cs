using Henke37.Win32.CdAccess;
using Henke37.Win32.DeviceEnum;
using Henke37.Win32.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CdControl {
	public partial class DriverForm : Form {

		private CdDrive cdDrive;

		private List<DeviceInterface> drives;
		private NativeFileNameConverter NameConverter;

		private const int WM_DEVICECHANGE = 0x0219;

		public DriverForm() {
			InitializeComponent();

			NameConverter = new NativeFileNameConverter();

			drives = CdDrive.GetCdDrives().ToList();
			DeviceInterface deviceInterface = drives[0];
			SelectDrive(deviceInterface);
		}

		private void SelectDrive(DeviceInterface deviceInterface) {
			cdDrive = new CdDrive(deviceInterface);
		}

		protected override void WndProc(ref Message m) {
			switch(m.Msg) {
				case WM_DEVICECHANGE:
					OnDeviceChange(m);
					break;
			}
			base.WndProc(ref m);
		}

		private void OnDeviceChange(Message m) {
			var dev= DevBroadcast.FromMessage(ref m);
			switch((int)m.WParam) {
				case DevBroadcast.DBT_DEVICEARRIVAL:
					if(IsSelectedDrive(dev)) {
						RebuildTrackList();
					}
					break;
				case DevBroadcast.DBT_DEVICEREMOVECOMPLETE:
					if(IsSelectedDrive(dev)) {
						ClearTrackList();
					}
					break;
			}
			
		}

		private bool IsSelectedDrive(DevBroadcast dev) {
			if(dev is DevBroadcastVolume vol) {
				return true;
			} else {
				return false;
			}
		}

		private void Eject_btn_Click(object sender, EventArgs e) {
			cdDrive.Eject();
		}

		private void Load_btn_Click(object sender, EventArgs e) {
			cdDrive.Load();
		}

		private void GetTOC_btn_Click(object sender, EventArgs e) {
			RebuildTrackList();
		}

		private void RebuildTrackList() {
			FullToc toc;
			int session = 1;

			cdDrive.VerifyMedia();

			try {
				track_lst.BeginUpdate();
				track_lst.Items.Clear();

				do {
					toc = cdDrive.GetFullTOC(session);
					foreach(var tocItem in toc.Entries) {
						if(tocItem.Point > 99) continue;

						var item = new ListViewItem(new string[] {
					tocItem.SessionNumber.ToString(),
					tocItem.Point.ToString(),
					tocItem.StartPosition.ToString()
				});
						item.Tag = tocItem;
						track_lst.Items.Add(item);
					}
				} while(session < toc.LastCompleteSession);

			} finally {
				track_lst.EndUpdate();
			}

			track_lst.Enabled = true;
		}

		private void ClearTrackList() {
			track_lst.Items.Clear();
			track_lst.Enabled = false;
		}
	}
}
