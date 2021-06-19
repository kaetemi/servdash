using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ServDash
{
	public partial class TerminalHost : UserControl
	{
		public TerminalHost()
		{
			Dock = DockStyle.Fill;
			InitializeComponent();
			writeOutputDelegate = new Action<string>(writeOutput);
		}

		public object ProcessObject { get; set; }

		bool shutdownRequested = false;

		public string WorkingDirectory { get; set; }
		public string LaunchCmd { get; set; }
		public string LaunchArgs { get; set; }
		public string ShutdownCmd { get; set; }
		public string ShutdownArgs { get; set; }
		public bool AutoRestart { get; set; }

		bool restartOnce = false;

		Process process;

		public event Action<TerminalHost> ProcessLaunched;
		public event Action<TerminalHost> ProcessStopping;
		public event Action<TerminalHost> ProcessStopped;

		public bool ProcessRunning { get { return process != null; } }

		public void Launch()
		{
			if (ProcessRunning)
			{
				return;
			}

			if (shutdownRequested)
			{
				Restart();
				return;
			}

			try
			{
				string cmd = LaunchCmd;
				if (!string.IsNullOrEmpty(WorkingDirectory))
					cmd = System.IO.Path.Combine(WorkingDirectory, cmd);
				ProcessStartInfo psi = new ProcessStartInfo(cmd, LaunchArgs);
				psi.RedirectStandardError = true;
				psi.RedirectStandardOutput = true;
				psi.CreateNoWindow = true;
				psi.UseShellExecute = false;
				if (!string.IsNullOrEmpty(WorkingDirectory))
					psi.WorkingDirectory = WorkingDirectory;
				captureProcess(Process.Start(psi));
			}
			catch (Exception exception)
			{
				output.AppendText("\r\n" + exception.ToString() + "\r\n");
			}
		}

		public void Restart()
		{
			if (shutdownRequested)
			{
				shutdownRequested = false;
				restartOnce = true;
				return;
			}

			Shutdown();
			shutdownRequested = false;
			restartOnce = true;
		}

		public void Shutdown()
		{

			if (process == null)
				return;

			if (process.HasExited)
				return;

			if (shutdownRequested)
			{
				process.Kill();
				return;
			}

			shutdownRequested = true;

			if (!string.IsNullOrEmpty(ShutdownCmd))
			{
				string cmd = ShutdownCmd;
				if (!string.IsNullOrEmpty(WorkingDirectory) && cmd.StartsWith("."))
					cmd = System.IO.Path.Combine(WorkingDirectory, cmd);
				ProcessStartInfo psi = new ProcessStartInfo(cmd, ShutdownArgs);
				psi.RedirectStandardError = true;
				psi.RedirectStandardOutput = true;
				psi.CreateNoWindow = true;
				psi.UseShellExecute = false;
				if (!string.IsNullOrEmpty(WorkingDirectory))
					psi.WorkingDirectory = WorkingDirectory;
				Process ps = Process.Start(psi);
				captureTerminal(ps);
			}
			else
			{
				process.Kill();
			}

			try
			{
				if (ProcessStopping != null)
				{
					ProcessStopping(this);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			try
			{
				if (process.HasExited)
					processExitedInvoked();
			} 
			catch (InvalidOperationException)
			{
				processExitedInvoked();
			}
		}

		void captureTerminal(Process process)
		{
			process.OutputDataReceived += outputDataReceived;
			process.ErrorDataReceived += errorDataReceived;
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
		}

		Action<string> writeOutputDelegate;
		void writeOutput(string data)
		{
			output.AppendText(data + "\r\n");
		}

		private void errorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
				Invoke(writeOutputDelegate, e.Data);
		}

		private void outputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
				Invoke(writeOutputDelegate, e.Data);
		}

		void captureProcess(Process process)
		{
			process.EnableRaisingEvents = true;
			captureTerminal(process);
			releaseProcess();
			this.process = process;

			output.AppendText("\r\n> Process Started\r\n");
			Refresh();

			try
			{
				try
				{
					if (ProcessLaunched != null)
					{
						ProcessLaunched(this);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
				if (process.HasExited)
					processExitedInvoked();
				else
					process.Exited += processExited;
			}
			catch (Exception exception)
			{
				output.AppendText("\r\n" + exception.ToString() + "\r\n");
				this.process = null;
			}
		}

		void releaseProcess()
		{
			if (process == null)
				return;

			try
			{
				process.Exited -= processExited;
				process.EnableRaisingEvents = false;
				output.AppendText("\r\n> Process Exited\r\n");
			}
			catch (Exception exception)
			{
				output.AppendText("\r\n" + exception.ToString() + "\r\n");
			}

			process = null;

			try
			{
				if (ProcessStopped != null)
				{
					ProcessStopped(this);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		private void processExited(object sender, EventArgs e)
		{
			Invoke(new MethodInvoker(processExitedInvoked));
		}

		private void processExitedInvoked()
		{
			if (process == null)
				return;

			releaseProcess();

			if ((AutoRestart || restartOnce) && !shutdownRequested)
				Launch();

			restartOnce = false;
			shutdownRequested = false;
		}

		private void TerminalHost_VisibleChanged(object sender, EventArgs e)
		{
			output.SelectionStart = output.TextLength;
			output.ScrollToCaret();
		}
	}
}
