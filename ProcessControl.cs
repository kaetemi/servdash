using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ServDash
{
	public partial class ProcessControl : UserControl
	{
		public ProcessControl()
		{
			InitializeComponent();
		}

		public object ProcessObject { get; set; }

		public event Action<ProcessControl> TitleClicked;
		public event Action<ProcessControl> LaunchClicked;
		public event Action<ProcessControl> StopClicked;

		bool started = false;

		public string Title { get { return titleButton.Text; } set { titleButton.Text = value; } }

		public void SetLaunched()
		{
			startButton.Image = Properties.Resources.control_stop_square;
			status.Image = Properties.Resources.status_away;
			started = true;
		}

		public void SetReady()
		{
			startButton.Image = Properties.Resources.control_stop_square;
			status.Image = Properties.Resources.status;
			started = true;
		}

		public void setStopped()
		{
			startButton.Image = Properties.Resources.control;
			status.Image = Properties.Resources.status_busy;
			started = false;
		}

		public void SetStopping()
		{
			startButton.Image = Properties.Resources.control_stop_square;
			status.Image = Properties.Resources.status_cyan;
			started = true;
		}

		private void titleButton_Click(object sender, EventArgs e)
		{
			if (TitleClicked != null)
				TitleClicked(this);
		}

		private void startButton_Click(object sender, EventArgs e)
		{
			if (!started)
			{
				if (LaunchClicked != null)
					LaunchClicked(this);
			}
			else
			{
				if (StopClicked != null)
					StopClicked(this);
			}
		}
	}
}
