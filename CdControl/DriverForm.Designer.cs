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
			this.load_btn = new System.Windows.Forms.Button();
			this.eject_btn = new System.Windows.Forms.Button();
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
			// DriverForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.eject_btn);
			this.Controls.Add(this.load_btn);
			this.Name = "DriverForm";
			this.Text = "Cd Test";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button load_btn;
		private System.Windows.Forms.Button eject_btn;
	}
}

