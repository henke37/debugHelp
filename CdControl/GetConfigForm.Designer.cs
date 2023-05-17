namespace CdControl {
	partial class GetConfigForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.props = new System.Windows.Forms.PropertyGrid();
			this.featureSelect = new System.Windows.Forms.ComboBox();
			this.requestTypeSelect = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// props
			// 
			this.props.Location = new System.Drawing.Point(12, 39);
			this.props.Name = "props";
			this.props.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.props.Size = new System.Drawing.Size(297, 426);
			this.props.TabIndex = 0;
			this.props.ToolbarVisible = false;
			// 
			// featureSelect
			// 
			this.featureSelect.FormattingEnabled = true;
			this.featureSelect.Location = new System.Drawing.Point(12, 12);
			this.featureSelect.Name = "featureSelect";
			this.featureSelect.Size = new System.Drawing.Size(121, 21);
			this.featureSelect.TabIndex = 1;
			this.featureSelect.SelectedIndexChanged += new System.EventHandler(this.featureSelect_SelectedIndexChanged);
			// 
			// requestTypeSelect
			// 
			this.requestTypeSelect.FormattingEnabled = true;
			this.requestTypeSelect.Items.AddRange(new object[] {
            "All",
            "Current",
            "One"});
			this.requestTypeSelect.Location = new System.Drawing.Point(155, 12);
			this.requestTypeSelect.Name = "requestTypeSelect";
			this.requestTypeSelect.Size = new System.Drawing.Size(121, 21);
			this.requestTypeSelect.TabIndex = 2;
			this.requestTypeSelect.SelectedIndexChanged += new System.EventHandler(this.requestTypeSelect_SelectedIndexChanged);
			// 
			// GetConfigForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(314, 467);
			this.Controls.Add(this.requestTypeSelect);
			this.Controls.Add(this.featureSelect);
			this.Controls.Add(this.props);
			this.Name = "GetConfigForm";
			this.Text = "GetConfigForm";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid props;
		private System.Windows.Forms.ComboBox featureSelect;
		private System.Windows.Forms.ComboBox requestTypeSelect;
	}
}