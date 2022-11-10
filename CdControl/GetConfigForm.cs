using Henke37.Win32.CdAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CdControl {
	public partial class GetConfigForm : Form {
		private CdDrive cdDrive;

		public GetConfigForm(CdDrive cdDrive) {
			this.cdDrive = cdDrive;
			InitializeComponent();

			featureSelect.Items.AddRange(Enum.GetNames(typeof(Configuration.FeatureNumber)));
		}

		private void featureSelect_SelectedIndexChanged(object sender, EventArgs e) {
			updateProps();
		}


		private void requestTypeSelect_SelectedIndexChanged(object sender, EventArgs e) {
			updateProps();
		}

		private void updateProps() {
			if(featureSelect.SelectedItem == null || requestTypeSelect.SelectedItem == null) {
				props.SelectedObject = null;
				return;
			}
			Configuration.FeatureNumber feature = (Configuration.FeatureNumber) Enum.Parse(typeof(Configuration.FeatureNumber), (string)featureSelect.SelectedItem);
			Configuration.RequestType requestType = (Configuration.RequestType) Enum.Parse(typeof(Configuration.RequestType), (string)requestTypeSelect.SelectedItem);

			var res=cdDrive.GetConfiguration(feature, requestType);

			props.SelectedObject=res;
		}
	}
}
