namespace CdControl {
	partial class DriverForm {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DriverForm));
			this.load_btn = new System.Windows.Forms.Button();
			this.eject_btn = new System.Windows.Forms.Button();
			this.GetTOC_btn = new System.Windows.Forms.Button();
			this.track_lst = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.listIcons = new System.Windows.Forms.ImageList(this.components);
			this.getCdTExt = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// load_btn
			// 
			this.load_btn.Location = new System.Drawing.Point(12, 12);
			this.load_btn.Name = "load_btn";
			this.load_btn.Size = new System.Drawing.Size(75, 23);
			this.load_btn.TabIndex = 0;
			this.load_btn.Text = "Load";
			this.load_btn.UseVisualStyleBackColor = true;
			this.load_btn.Click += new System.EventHandler(this.Load_btn_Click);
			// 
			// eject_btn
			// 
			this.eject_btn.Location = new System.Drawing.Point(93, 12);
			this.eject_btn.Name = "eject_btn";
			this.eject_btn.Size = new System.Drawing.Size(75, 23);
			this.eject_btn.TabIndex = 1;
			this.eject_btn.Text = "Eject";
			this.eject_btn.UseVisualStyleBackColor = true;
			this.eject_btn.Click += new System.EventHandler(this.Eject_btn_Click);
			// 
			// GetTOC_btn
			// 
			this.GetTOC_btn.Location = new System.Drawing.Point(174, 12);
			this.GetTOC_btn.Name = "GetTOC_btn";
			this.GetTOC_btn.Size = new System.Drawing.Size(75, 23);
			this.GetTOC_btn.TabIndex = 2;
			this.GetTOC_btn.Text = "GetTOC";
			this.GetTOC_btn.UseVisualStyleBackColor = true;
			this.GetTOC_btn.Click += new System.EventHandler(this.GetTOC_btn_Click);
			// 
			// track_lst
			// 
			this.track_lst.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
			this.track_lst.Enabled = false;
			this.track_lst.HideSelection = false;
			this.track_lst.Location = new System.Drawing.Point(12, 42);
			this.track_lst.Name = "track_lst";
			this.track_lst.Size = new System.Drawing.Size(575, 396);
			this.track_lst.SmallImageList = this.listIcons;
			this.track_lst.TabIndex = 3;
			this.track_lst.UseCompatibleStateImageBehavior = false;
			this.track_lst.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Session";
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Track";
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Start Time";
			// 
			// listIcons
			// 
			this.listIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("listIcons.ImageStream")));
			this.listIcons.TransparentColor = System.Drawing.Color.Transparent;
			this.listIcons.Images.SetKeyName(0, "data");
			this.listIcons.Images.SetKeyName(1, "audio");
			// 
			// getCdTExt
			// 
			this.getCdTExt.Location = new System.Drawing.Point(256, 12);
			this.getCdTExt.Name = "getCdTExt";
			this.getCdTExt.Size = new System.Drawing.Size(75, 23);
			this.getCdTExt.TabIndex = 4;
			this.getCdTExt.Text = "GetCdText";
			this.getCdTExt.UseVisualStyleBackColor = true;
			this.getCdTExt.Click += new System.EventHandler(this.getCdTExt_Click);
			// 
			// DriverForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(599, 450);
			this.Controls.Add(this.getCdTExt);
			this.Controls.Add(this.track_lst);
			this.Controls.Add(this.GetTOC_btn);
			this.Controls.Add(this.eject_btn);
			this.Controls.Add(this.load_btn);
			this.Name = "DriverForm";
			this.Text = "Cd Test";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button load_btn;
		private System.Windows.Forms.Button eject_btn;
		private System.Windows.Forms.Button GetTOC_btn;
		private System.Windows.Forms.ListView track_lst;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ImageList listIcons;
		private System.Windows.Forms.Button getCdTExt;
	}
}

