
namespace ServDash
{
	partial class ProcessControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.titleButton = new System.Windows.Forms.Button();
			this.startButton = new System.Windows.Forms.Button();
			this.status = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.status)).BeginInit();
			this.SuspendLayout();
			// 
			// titleButton
			// 
			this.titleButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.titleButton.Location = new System.Drawing.Point(54, 0);
			this.titleButton.Name = "titleButton";
			this.titleButton.Size = new System.Drawing.Size(93, 23);
			this.titleButton.TabIndex = 0;
			this.titleButton.Text = "...";
			this.titleButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.titleButton.UseVisualStyleBackColor = true;
			this.titleButton.Click += new System.EventHandler(this.titleButton_Click);
			// 
			// startButton
			// 
			this.startButton.Image = global::ServDash.Properties.Resources.control;
			this.startButton.Location = new System.Drawing.Point(27, 0);
			this.startButton.Name = "startButton";
			this.startButton.Size = new System.Drawing.Size(23, 23);
			this.startButton.TabIndex = 2;
			this.startButton.UseVisualStyleBackColor = true;
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// status
			// 
			this.status.Image = global::ServDash.Properties.Resources.status_offline;
			this.status.Location = new System.Drawing.Point(0, 0);
			this.status.Name = "status";
			this.status.Padding = new System.Windows.Forms.Padding(1, 1, 0, 0);
			this.status.Size = new System.Drawing.Size(23, 23);
			this.status.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.status.TabIndex = 1;
			this.status.TabStop = false;
			// 
			// ProcessControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.startButton);
			this.Controls.Add(this.status);
			this.Controls.Add(this.titleButton);
			this.Name = "ProcessControl";
			this.Size = new System.Drawing.Size(150, 23);
			((System.ComponentModel.ISupportInitialize)(this.status)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button titleButton;
		private System.Windows.Forms.PictureBox status;
		private System.Windows.Forms.Button startButton;
	}
}
