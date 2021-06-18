
namespace ServDash
{
	partial class TerminalHost
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
			this.output = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// output
			// 
			this.output.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.output.Dock = System.Windows.Forms.DockStyle.Fill;
			this.output.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
			this.output.Location = new System.Drawing.Point(0, 0);
			this.output.Multiline = true;
			this.output.Name = "output";
			this.output.ReadOnly = true;
			this.output.Size = new System.Drawing.Size(150, 150);
			this.output.TabIndex = 1;
			// 
			// TerminalHost
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.output);
			this.Name = "TerminalHost";
			this.VisibleChanged += new System.EventHandler(this.TerminalHost_VisibleChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox output;
	}
}
