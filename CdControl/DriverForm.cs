using Henke37.Win32.CdAccess;
using Henke37.Win32.DeviceEnum;
using Henke37.Win32.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

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

			cdDrive.VerifyMedia();

			var sessions = cdDrive.GetSessionData();

			try {
				AlbumTitle.Text = "";
				track_lst.BeginUpdate();
				track_lst.Items.Clear();

				toc = cdDrive.GetFullTOC(1);
				var cdText = cdDrive.GetCdText(1);

				if(cdText!=null) {
					var titleInfo = cdText.infos.Find(i => (i.TrackNr == 0) && (i.Type == CdTextBlockType.AlbumNameOrTrackTitle));
					if(titleInfo != null) AlbumTitle.Text = titleInfo.Text;
				}

				foreach(var tocItem in toc.Entries) {
					if(tocItem.Point > 99) continue;

					string title = "";
					if(cdText != null) {
						var titleInfo = cdText.infos.Find(i => (i.TrackNr == tocItem.Point) && (i.Type == CdTextBlockType.AlbumNameOrTrackTitle));
						if(titleInfo != null) {
							title = titleInfo.Text;
						}
					}

					var item = new ListViewItem(new string[] {
						tocItem.SessionNumber.ToString(),
						tocItem.Point.ToString(),
						tocItem.StartPosition.ToString(),
						title
					});
					item.ImageKey = (tocItem.IsAudio ? "audio" : "data");
					item.Tag = tocItem;
					track_lst.Items.Add(item);
				}

			} finally {
				track_lst.EndUpdate();
			}

			track_lst.Enabled = true;
		}

		private void ClearTrackList() {
			track_lst.Items.Clear();
			track_lst.Enabled = false;
		}

		private void getConfig_Click(object sender, EventArgs e) {
			new GetConfigForm(cdDrive).ShowDialog(this);
		}
	}
}
