using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ServDash
{
	public enum ProcessState
	{
		New,
		Launched,
		Captured,
		Ready,
		Stopping,
		Stopped
	}

	public partial class ProcessControl : UserControl
	{
		public ProcessControl()
		{
			InitializeComponent();
			State = ProcessState.New;
		}

		public object ProcessObject { get; set; }

		public event Action<ProcessControl> TitleClicked;
		public event Action<ProcessControl> LaunchClicked;
		public event Action<ProcessControl> StopClicked;

		bool started = false;

		public string Title { get { return titleButton.Text; } set { titleButton.Text = value; } }

		public ProcessState State { get; set; }

		public void SetLaunched()
		{
			startButton.Image = Properties.Resources.control_stop_square;
			status.Image = Properties.Resources.status_orange;
			State = ProcessState.Launched;
			started = true;
		}

		public void SetCaptured()
		{
			startButton.Image = Properties.Resources.control_stop_square;
			status.Image = Properties.Resources.status_away;
			State = ProcessState.Captured;
			started = true;
		}

		public void SetReady()
		{
			startButton.Image = Properties.Resources.control_stop_square;
			status.Image = Properties.Resources.status;
			State = ProcessState.Ready;
			started = true;
		}

		public void setStopped()
		{
			startButton.Image = Properties.Resources.control;
			status.Image = Properties.Resources.status_busy;
			State = ProcessState.Stopped;
			started = false;
		}

		public void SetStopping()
		{
			startButton.Image = Properties.Resources.control_stop_square;
			status.Image = Properties.Resources.status_blue;
			State = ProcessState.Stopping;
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
