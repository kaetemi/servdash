using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ServDash
{
	public partial class MainWindow : Form
	{
		public MainWindow()
		{
			Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
			InitializeComponent();
			test();
			test();
			test();
			test2();

			// TODO: ProcessTerminal.cs (terminal output, shutdown cmd, 
		}

		void test2()
		{
			foreach (Control c in splitContainer.Panel2.Controls)
				c.Visible = false;
			TerminalHost host = new TerminalHost();
			ProcessControl control = new ProcessControl();
			host.ProcessObject = control;
			control.ProcessObject = host;
			host.LaunchCmd = "cmd";
			host.LaunchArgs = "/C dir";
			host.ProcessLaunched += terminalLaunched;
			host.ProcessStopped += terminalStopped;
			host.ProcessStopping += terminalStopping;
			splitContainer.Panel2.Controls.Add(host);
			control.Location = new Point(5, splitContainer.Panel1.Controls.Count * (27) + 7 + 2);
			control.Size = new Size(splitContainer.Panel1.ClientSize.Width - 5, 23);
			control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			control.Title = host.LaunchCmd;
			control.TitleClicked += titleClicked;
			control.LaunchClicked += launchClicked;
			control.StopClicked += stopClicked;
			splitContainer.Panel1.Controls.Add(control);
		}

		void test()
		{
			foreach (Control c in splitContainer.Panel2.Controls)
				c.Visible = false;
			ProcessHost host = new ProcessHost();
			ProcessControl control = new ProcessControl();
			host.ProcessObject = control;
			control.ProcessObject = host;
			host.LaunchCmd = "notepad";
			host.ReadyPattern = @"^[^Untitled].+ Note";
			host.ProcessTitleChanged += processTitleChanged;
			host.ProcessLaunched += processLaunched;
			host.ProcessReady +=  processReady;
			host.ProcessStopped += processStopped;
			host.ProcessStopping += processStopping;
			splitContainer.Panel2.Controls.Add(host);
			control.Location = new Point(5, splitContainer.Panel1.Controls.Count * (27) + 7 + 2);
			control.Size = new Size(splitContainer.Panel1.ClientSize.Width - 5, 23);
			control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			control.Title = host.LaunchCmd;
			control.TitleClicked += titleClicked;
			control.LaunchClicked += launchClicked;
			control.StopClicked += stopClicked;
			splitContainer.Panel1.Controls.Add(control);
		}

		private void stopClicked(ProcessControl control)
		{
			Control host = (Control)control.ProcessObject;
			if (host is ProcessHost)
			{
				(host as ProcessHost).Shutdown();
			}
			else if (host is TerminalHost)
			{
				(host as TerminalHost).Shutdown();
			}
		}

		private void launchClicked(ProcessControl control)
		{
			Control host = (Control)control.ProcessObject;
			foreach (Control c in splitContainer.Panel2.Controls)
				if (c != host)
					c.Visible = false;
			host.Visible = true;
			if (host is ProcessHost)
			{
				(host as ProcessHost).Launch();
			}
			else if (host is TerminalHost)
			{
				(host as TerminalHost).Launch();
			}
		}

		private void titleClicked(ProcessControl control)
		{
			Control host = (Control)control.ProcessObject;
			host.Visible = true;
			foreach (Control c in splitContainer.Panel2.Controls)
				if (c != host)
					c.Visible = false;
			Text = control.Title;
		}

		private void terminalStopped(TerminalHost host)
		{
			ProcessControl control = (ProcessControl)host.ProcessObject;
			control.setStopped();
			processStopped();
		}

		private void terminalStopping(TerminalHost host)
		{
			ProcessControl control = (ProcessControl)host.ProcessObject;
			control.SetStopping();
		}

		private void terminalLaunched(TerminalHost host)
		{
			ProcessControl control = (ProcessControl)host.ProcessObject;
			control.SetLaunched();
			control.SetReady();
		}

		private void processStopped(ProcessHost host)
		{
			ProcessControl control = (ProcessControl)host.ProcessObject;
			control.setStopped();
			processStopped();
		}

		void processStopped()
		{
			if (closingWindow)
			{
				foreach (Control c in splitContainer.Panel2.Controls)
				{
					if (c is ProcessHost)
					{
						ProcessHost checkHost = (ProcessHost)c;
						if (checkHost.ProcessRunning)
							return;
					}
					else if (c is TerminalHost)
					{
						TerminalHost checkHost = (TerminalHost)c;
						if (checkHost.ProcessRunning)
							return;
					}
				}
				closingWindow = false;
				Close();
			}
		}

		private void processStopping(ProcessHost host)
		{
			ProcessControl control = (ProcessControl)host.ProcessObject;
			control.SetStopping();
		}

		private void processReady(ProcessHost host)
		{
			ProcessControl control = (ProcessControl)host.ProcessObject;
			control.SetReady();
		}

		private void processLaunched(ProcessHost host)
		{
			ProcessControl control = (ProcessControl)host.ProcessObject;
			control.SetLaunched();
		}

		private void processTitleChanged(ProcessHost host, string title)
		{
			ProcessControl control = (ProcessControl)host.ProcessObject;
			control.Title = title;
			if (host.Visible)
				Text = title;
		}

		bool closingWindow = false;

		private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			closingWindow = true;
			foreach (Control c in splitContainer.Panel2.Controls)
			{
				if (c is ProcessHost)
				{
					ProcessHost checkHost = (ProcessHost)c;
					if (checkHost.ProcessRunning)
					{
						checkHost.Shutdown();
						e.Cancel = true;
					}
				}
				else if (c is TerminalHost)
				{
					TerminalHost checkHost = (TerminalHost)c;
					if (checkHost.ProcessRunning)
					{
						checkHost.Shutdown();
						e.Cancel = true;
					}
				}
			}
		}

		private void launchAll_Click(object sender, EventArgs e)
		{
			foreach (Control c in splitContainer.Panel2.Controls)
			{
				if (c is ProcessHost)
				{
					(c as ProcessHost).Launch();
				}
				else if (c is TerminalHost)
				{
					(c as TerminalHost).Launch();
				}
			}
		}

		private void shutdownAll_Click(object sender, EventArgs e)
		{
			foreach (Control c in splitContainer.Panel2.Controls)
			{
				if (c is ProcessHost)
				{
					(c as ProcessHost).Shutdown();
				}
				else if (c is TerminalHost)
				{
					(c as TerminalHost).Shutdown();
				}
			}
		}
	}
}
