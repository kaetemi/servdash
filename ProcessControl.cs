using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

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
			Priority = int.MaxValue;
			LaunchCtrlValues = new string[4];
		}

		public object ProcessObject { get; set; }

		public event Action<ProcessControl> TitleClicked;
		public event Action<ProcessControl> LaunchClicked;
		public event Action<ProcessControl> StopClicked;

		public event Action<ProcessControl> LaunchWanted;

		bool started = false;

		public string Title { get { return titleButton.Text; } set { titleButton.Text = value; } }

		public bool StaticTitle { get; set; }

		public string ProcessName { get; set; }

		public ProcessState State { get; set; }

		public int Priority { get; set; }

		public string LaunchCtrlFile { get; set; }

		public string LaunchStateFile { get; set; }

		public FileSystemWatcher LaunchCtrlWatcher { get; set; }

		public string[] LaunchCtrlValues { get; set; }

		public int LaunchCtrlValue { get; set; }

		public int GetLaunchCtrlValue(string value)
		{
			for (int i = 0; i < 4; ++i)
			{
				if (value == LaunchCtrlValues[i])
					return i;
			}
			if (State == ProcessState.Stopping)
				return 2;
			if (State == ProcessState.Stopped)
				return 3;
			if (State == ProcessState.Launched
				|| State == ProcessState.Captured
				|| State == ProcessState.Ready)
				return 1;
			return 0;
		}

		public void LaunchCtrlChanged(object sender, FileSystemEventArgs e)
		{
			BeginInvoke(new MethodInvoker(launchCtrlChanged));
		}

		void launchCtrlChanged()
		{
			try
			{
				string launchValue = File.ReadAllText(LaunchCtrlFile);
				int value = GetLaunchCtrlValue(launchValue);
				if (value != LaunchCtrlValue)
				{
					if ((value == 0) && !started && LaunchWanted != null)
						LaunchWanted(this);
					else if ((value == 2) && State != ProcessState.Stopping && started && StopClicked != null)
						StopClicked(this);
					else
						syncLaunchState();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public void Launch()
		{
			started = true;
			if (LaunchClicked != null)
				LaunchClicked(this);
		}

		void syncLaunchState()
		{
			try
			{
				if (LaunchStateFile != null)
				{
					string launchValue = File.ReadAllText(LaunchStateFile);
					if (launchValue != LaunchCtrlValues[LaunchCtrlValue]
						&& (LaunchCtrlValue == 1 || LaunchCtrlValue == 3))
					{
						File.WriteAllText(LaunchStateFile, LaunchCtrlValues[LaunchCtrlValue]);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public void SetLaunched()
		{
			startButton.Image = Properties.Resources.control_stop_square;
			status.Image = Properties.Resources.status_orange;
			State = ProcessState.Launched;
			LaunchCtrlValue = 1;
			syncLaunchState();
			started = true;
		}

		public void SetCaptured()
		{
			startButton.Image = Properties.Resources.control_stop_square;
			status.Image = Properties.Resources.status_away;
			State = ProcessState.Captured;
			LaunchCtrlValue = 1;
			syncLaunchState();
			started = true;
		}

		public void SetReady()
		{
			startButton.Image = Properties.Resources.control_stop_square;
			status.Image = Properties.Resources.status;
			State = ProcessState.Ready;
			LaunchCtrlValue = 1;
			syncLaunchState();
			started = true;
		}

		public void setStopped()
		{
			startButton.Image = Properties.Resources.control;
			status.Image = Properties.Resources.status_busy;
			State = ProcessState.Stopped;
			LaunchCtrlValue = 3;
			syncLaunchState();
			started = false;
		}

		public void SetStopping()
		{
			startButton.Image = Properties.Resources.control_stop_square;
			status.Image = Properties.Resources.status_blue;
			State = ProcessState.Stopping;
			LaunchCtrlValue = 2;
			syncLaunchState();
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
				if (LaunchCtrlFile != null)
				{
					File.WriteAllText(LaunchCtrlFile, LaunchCtrlValues[0]);
				}
				else
				{
					Launch();
				}
			}
			else
			{
				if (LaunchCtrlFile != null)
				{
					File.WriteAllText(LaunchCtrlFile, LaunchCtrlValues[2]);
				}
				else
				{
					if (StopClicked != null)
						StopClicked(this);
				}
			}
		}
	}
}
