
namespace ServDash
{
	partial class MainWindow
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.shutdownAll = new System.Windows.Forms.Button();
			this.launchAll = new System.Windows.Forms.Button();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.AutoScroll = true;
			this.splitContainer.Panel1.Controls.Add(this.shutdownAll);
			this.splitContainer.Panel1.Controls.Add(this.launchAll);
			this.splitContainer.Panel1MinSize = 180;
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.splitContainer.Size = new System.Drawing.Size(1064, 501);
			this.splitContainer.SplitterDistance = 180;
			this.splitContainer.TabIndex = 0;
			// 
			// shutdownAll
			// 
			this.shutdownAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.shutdownAll.Image = global::ServDash.Properties.Resources.control_stop_square;
			this.shutdownAll.Location = new System.Drawing.Point(7, 34);
			this.shutdownAll.Name = "shutdownAll";
			this.shutdownAll.Size = new System.Drawing.Size(170, 23);
			this.shutdownAll.TabIndex = 1;
			this.shutdownAll.Text = "Shutdown All";
			this.shutdownAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.shutdownAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.shutdownAll.UseVisualStyleBackColor = true;
			this.shutdownAll.Click += new System.EventHandler(this.shutdownAll_Click);
			// 
			// launchAll
			// 
			this.launchAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.launchAll.Image = global::ServDash.Properties.Resources.control;
			this.launchAll.Location = new System.Drawing.Point(7, 7);
			this.launchAll.Name = "launchAll";
			this.launchAll.Size = new System.Drawing.Size(170, 23);
			this.launchAll.TabIndex = 0;
			this.launchAll.Text = "Launch All";
			this.launchAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.launchAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.launchAll.UseVisualStyleBackColor = true;
			this.launchAll.Click += new System.EventHandler(this.launchAll_Click);
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1064, 501);
			this.Controls.Add(this.splitContainer);
			this.Name = "MainWindow";
			this.Text = "Service Dashboard";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
			this.Shown += new System.EventHandler(this.MainWindow_Shown);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.Button launchAll;
		private System.Windows.Forms.Button shutdownAll;
	}
}

