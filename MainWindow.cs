﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace ServDash
{
	public partial class MainWindow : Form
	{
		string titlePrefix = "Service Dashboard";

		Dictionary<string, ProcessControl> NamedProcesses = new Dictionary<string, ProcessControl>();
		LinkedList<string> LaunchProcesses = new LinkedList<string>();

		public MainWindow()
		{
			Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
			InitializeComponent();
			/*
			test();
			test();
			test();
			test2();
			return;
			*/
			string[] args = Environment.GetCommandLineArgs();
			if (args.Length > 1)
			{
				string filename = args[1];
				Environment.CurrentDirectory = new System.IO.FileInfo(filename).DirectoryName;
				Dictionary<string, Dictionary<string, string>> ini = IniReader.Read(filename);
				if (ini[""].ContainsKey("Title"))
					titlePrefix = ini[""]["Title"];
				Text = titlePrefix;
				string firstName = null;
				foreach (string name in ini.Keys)
				{
					if (name == "")
						continue;

					firstName = name;
					Dictionary<string, string> section = ini[name];
					ProcessControl control = new ProcessControl();
					string launchCmd;
					string workingDirectory = null;
					bool autoLaunch = true;
					if (section.ContainsKey("Terminal") && section["Terminal"] == "1")
					{
						TerminalHost host = new TerminalHost();
						host.ProcessObject = control;
						control.ProcessObject = host;
						if (section.ContainsKey("LaunchCmd"))
							host.LaunchCmd = section["LaunchCmd"];
						if (section.ContainsKey("LaunchArgs"))
							host.LaunchArgs = section["LaunchArgs"];
						if (section.ContainsKey("WorkingDirectory"))
						{
							host.WorkingDirectory = section["WorkingDirectory"];
							workingDirectory = host.WorkingDirectory;
						}
						if (section.ContainsKey("ShutdownCmd"))
							host.ShutdownCmd = section["ShutdownCmd"];
						if (section.ContainsKey("ShutdownArgs"))
							host.ShutdownArgs = section["ShutdownArgs"];
						host.ProcessLaunched += terminalLaunched;
						host.ProcessStopped += terminalStopped;
						host.ProcessStopping += terminalStopping;
						splitContainer.Panel2.Controls.Add(host);
						launchCmd = host.LaunchCmd;
						host.Visible = false;
					}
					else
					{
						ProcessHost host = new ProcessHost();
						host.ProcessObject = control;
						control.ProcessObject = host;
						if (section.ContainsKey("LaunchCmd"))
							host.LaunchCmd = section["LaunchCmd"];
						if (section.ContainsKey("LaunchArgs"))
							host.LaunchArgs = section["LaunchArgs"];
						if (section.ContainsKey("WorkingDirectory"))
						{
							host.WorkingDirectory = section["WorkingDirectory"];
							workingDirectory = host.WorkingDirectory;
						}
						if (section.ContainsKey("ReadyPattern"))
							host.ReadyPattern = section["ReadyPattern"];
						host.ProcessTitleChanged += processTitleChanged;
						host.ProcessLaunched += processLaunched;
						host.ProcessCaptured += processCaptured;
						host.ProcessReady += processReady;
						host.ProcessStopped += processStopped;
						host.ProcessStopping += processStopping;
						splitContainer.Panel2.Controls.Add(host);
						launchCmd = host.LaunchCmd;
						host.Visible = false;
					}
					control.Title = launchCmd;
					control.ProcessName = name;
					if (section.ContainsKey("Title"))
					{
						control.Title = section["Title"];
						control.StaticTitle = true;
					}
					if (section.ContainsKey("Priority"))
						control.Priority = int.Parse(section["Priority"]);
					if (section.ContainsKey("AutoStart") && section["AutoStart"] == "0")
						autoLaunch = false;
					if (section.ContainsKey("LaunchCtrl"))
					{
						string[] launchCtrl = section["LaunchCtrl"].Split(new char[] { ',' });
						string ctrlFile = launchCtrl[0];
						if (!string.IsNullOrEmpty(workingDirectory))
							ctrlFile = Path.Combine(workingDirectory, ctrlFile);
						FileInfo ctrlInfo = new FileInfo(ctrlFile);
						control.LaunchCtrlFile = ctrlFile;
						string stateFile = launchCtrl[1];
						if (!string.IsNullOrEmpty(workingDirectory))
							stateFile = Path.Combine(workingDirectory, stateFile);
						FileInfo stateInfo = new FileInfo(stateFile);
						control.LaunchStateFile = stateFile;
						control.LaunchCtrlValues[0] = launchCtrl[2];
						control.LaunchCtrlValues[1] = launchCtrl[3];
						control.LaunchCtrlValues[2] = launchCtrl[4];
						control.LaunchCtrlValues[3] = launchCtrl[5];
						string launchValue;
						try
						{
							if (!ctrlInfo.Exists)
							{
								launchValue = autoLaunch ? control.LaunchCtrlValues[0] : control.LaunchCtrlValues[2];
								File.WriteAllText(ctrlFile, launchValue);
							}
							else
							{
								launchValue = File.ReadAllText(ctrlFile);
							}
							control.LaunchCtrlValue = 3;
							autoLaunch = (control.GetLaunchCtrlValue(launchValue) == 0);
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex);
						}
						try
						{
							File.WriteAllText(stateFile, control.LaunchCtrlValues[3]);
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex);
						}
						control.LaunchCtrlWatcher = new FileSystemWatcher(ctrlInfo.Directory.FullName, ctrlInfo.Name);
						control.LaunchCtrlWatcher.Changed += control.LaunchCtrlChanged;
						control.LaunchCtrlWatcher.EnableRaisingEvents = true;
					}
					control.Location = new Point(5, splitContainer.Panel1.Controls.Count * (27) + 7 + 2);
					control.Size = new Size(splitContainer.Panel1.ClientSize.Width - 5 - 1, 23);
					control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
					control.TitleClicked += titleClicked;
					control.LaunchClicked += launchClicked;
					control.StopClicked += stopClicked;
					control.LaunchWanted += launchWanted;
					splitContainer.Panel1.Controls.Add(control);
					NamedProcesses[name] = control;
					if (autoLaunch)
						LaunchProcesses.AddLast(name);
				}
				if (!string.IsNullOrEmpty(firstName))
					titleClicked(NamedProcesses[firstName]);
			}
		}

		private void launchWanted(ProcessControl control)
		{
			if (closingWindow)
				return;

			if (control.ProcessName == null)
			{
				control.Launch();
			}
			else
			{
				if (!LaunchProcesses.Contains(control.ProcessName))
				{
					LaunchProcesses.AddFirst(control.ProcessName);
					autoStart();
				}
			}
		}

		private void MainWindow_Shown(object sender, EventArgs e)
		{
			autoStart();
		}

		bool autoStarting = false;
		bool autoStartLoop = false;

		void autoStart()
		{
			if (closingWindow)
				return;

			if (autoStarting)
			{
				autoStartLoop = true;
				return;
			}

			foreach (ProcessControl control in NamedProcesses.Values)
			{
				if (control.State == ProcessState.Launched && control.Priority == int.MaxValue)
					return; // Only launch one at a time
			}

			int lowestPriority = int.MaxValue;

			foreach (ProcessControl control in NamedProcesses.Values)
			{
				if ((((control.State == ProcessState.Stopped
					|| control.State == ProcessState.New) && LaunchProcesses.Contains(control.ProcessName))
					|| control.State == ProcessState.Launched
					|| control.State == ProcessState.Captured
					|| control.State == ProcessState.Stopping)
					&& (control.Priority < lowestPriority))
				{
					lowestPriority = control.Priority;
				}
			}

			foreach (string name in LaunchProcesses)
			{
				ProcessControl control = NamedProcesses[name];
				if ((control.State == ProcessState.Stopped
					|| control.State == ProcessState.New)
					&& (control.Priority <= lowestPriority))
				{
					autoStarting = true;
					try
					{
						control.Launch();
						LaunchProcesses.Remove(name);
					}
					finally
					{
						autoStarting = false;
						if (autoStartLoop)
						{
							autoStartLoop = false;
							autoStart();
						}
					}
					return;
				}
			}
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
			control.Size = new Size(splitContainer.Panel1.ClientSize.Width - 5 - 1, 23);
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
			host.ProcessCaptured += processCaptured;
			host.ProcessReady +=  processReady;
			host.ProcessStopped += processStopped;
			host.ProcessStopping += processStopping;
			splitContainer.Panel2.Controls.Add(host);
			control.Location = new Point(5, splitContainer.Panel1.Controls.Count * (27) + 7 + 2);
			control.Size = new Size(splitContainer.Panel1.ClientSize.Width - 5 - 1, 23);
			control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			control.Title = host.LaunchCmd;
			control.TitleClicked += titleClicked;
			control.LaunchClicked += launchClicked;
			control.StopClicked += stopClicked;
			splitContainer.Panel1.Controls.Add(control);
		}

		private void stopClicked(ProcessControl control)
		{
			if (control.ProcessName != null && LaunchProcesses.Contains(control.ProcessName))
			{
				LaunchProcesses.Remove(control.ProcessName);
			}
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
			if (closingWindow)
				return;

			Control host = (Control)control.ProcessObject;
			foreach (Control c in splitContainer.Panel2.Controls)
				if (c != host)
					c.Visible = false;
			foreach (Control c in splitContainer.Panel1.Controls)
				if (c != control && c is ProcessControl)
					c.BackColor = SystemColors.Control;
			host.Visible = true;
			control.BackColor = SystemColors.Highlight;
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
			control.BackColor = SystemColors.Highlight;
			foreach (Control c in splitContainer.Panel1.Controls)
				if (c != control && c is ProcessControl)
					c.BackColor = SystemColors.Control;
			foreach (Control c in splitContainer.Panel2.Controls)
				if (c != host)
					c.Visible = false;
			Text = titlePrefix + " - " + control.ProcessTitle;
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
			autoStart();
		}

		private void terminalLaunched(TerminalHost host)
		{
			ProcessControl control = (ProcessControl)host.ProcessObject;
			control.SetLaunched();
			control.SetCaptured();
			control.SetReady();
			autoStart();
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
			else
			{
				autoStart();
			}
		}

		private void processStopping(ProcessHost host)
		{
			ProcessControl control = (ProcessControl)host.ProcessObject;
			control.SetStopping();
			autoStart();
		}

		private void processReady(ProcessHost host)
		{
			ProcessControl control = (ProcessControl)host.ProcessObject;
			control.SetReady();
			autoStart();
		}

		private void processCaptured(ProcessHost host)
		{
			ProcessControl control = (ProcessControl)host.ProcessObject;
			control.SetCaptured();
			autoStart();
		}

		private void processLaunched(ProcessHost host)
		{
			ProcessControl control = (ProcessControl)host.ProcessObject;
			control.SetLaunched();
			autoStart();
		}

		private void processTitleChanged(ProcessHost host, string title)
		{
			ProcessControl control = (ProcessControl)host.ProcessObject;
			if (!control.StaticTitle)
				control.Title = title;
			control.ProcessTitle = title;
			if (host.Visible)
				Text = titlePrefix + " - " + title;
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
			if (closingWindow)
				return;

			if (NamedProcesses.Count > 0)
			{
				LaunchProcesses.Clear();
				foreach (string name in NamedProcesses.Keys)
				{
					LaunchProcesses.AddLast(name);
				}
				autoStart();
			}
			else
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
		}

		private void shutdownAll_Click(object sender, EventArgs e)
		{
			LaunchProcesses.Clear();
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
