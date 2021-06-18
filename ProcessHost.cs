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
	public partial class ProcessHost : UserControl
	{
		public ProcessHost()
		{
			Dock = DockStyle.Fill;
			InitializeComponent();
			processObjectNameChangeDelegate = new Win32.WinEventDelegate(processObjectNameChange);
		}

		public object ProcessObject { get; set; }

		bool shutdownRequested = false;

		string readyPatternString;
		Regex readyPattern;
		bool readyTriggered = false;

		public string WorkingDirectory { get; set; }
		public string LaunchCmd { get; set; }
		public string LaunchArgs { get; set; }
		public string ReadyPattern 
		{
			get 
			{ 
				return readyPatternString;
			}
			set
			{
				readyPatternString = value;
				if (!string.IsNullOrEmpty(value))
					readyPattern = new Regex(value, RegexOptions.Compiled);
			}
		}
		public bool AutoRestart { get; set; }

		bool restartOnce = false;

		Process process;
		IntPtr mainWindowHandle;
		IntPtr processNameChangeHook;

		public event Action<ProcessHost> ProcessLaunched;
		public event Action<ProcessHost> ProcessReady;
		public event Action<ProcessHost> ProcessStopping;
		public event Action<ProcessHost> ProcessStopped;

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
				if (!string.IsNullOrEmpty(WorkingDirectory))
					psi.WorkingDirectory = WorkingDirectory;
				captureProcess(Process.Start(psi));
			}
			catch (Exception exception)
			{
				statusLabel.Text = exception.ToString();
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

			process.CloseMainWindow();

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
		}

		void captureProcess(Process process)
		{
			releaseProcess();
			this.process = process;
			process.EnableRaisingEvents = true;

			statusLabel.Text = "Starting...";
			statusLabel.Visible = true;
			readyTriggered = false;
			Refresh();

			try
			{
				while (process.MainWindowHandle == IntPtr.Zero)
				{
					process.WaitForInputIdle();
					process.Refresh();
				}
				try
				{
					if (ProcessLaunched != null)
					{
						ProcessLaunched(this);
					}
					if (string.IsNullOrEmpty(ReadyPattern) && ProcessReady != null)
					{
						readyTriggered = true;
						ProcessReady(this);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
				processNameChangeHook = Win32.SetWinEventHook(Win32.WinEvents.EVENT_OBJECT_NAMECHANGE, Win32.WinEvents.EVENT_OBJECT_NAMECHANGE, 
					IntPtr.Zero, processObjectNameChangeDelegate, 
					(uint)process.Id, 0, Win32.WinEventFlags.WINEVENT_OUTOFCONTEXT);
				captureMainWindow();
				if (process.HasExited)
					processExitedInvoked();
				else
					process.Exited += processExited;
			}
			catch (Exception exception)
			{
				statusLabel.Text = exception.ToString();
				this.process = null;
			}

			statusLabel.Visible = false;
			SetStyle(ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint, true);
		}

		void releaseProcess()
		{
			if (process == null)
				return;

			if (processNameChangeHook != IntPtr.Zero)
			{
				Win32.UnhookWinEvent(processNameChangeHook);
				processNameChangeHook = IntPtr.Zero;
			}

			bool hadWindow = mainWindowHandle != IntPtr.Zero;

			try
			{
				process.Exited -= processExited;
				process.EnableRaisingEvents = false;
				SetStyle(ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint, false);
				statusLabel.Text = "";
				statusLabel.Visible = true;

				releaseMainWindow();
			}
			catch (Exception exception)
			{
				statusLabel.Text = exception.ToString();
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

			if (hadWindow)
			{
				Refresh();
			}
		}

		public delegate void TitleChangedDelegate(ProcessHost host, string title);
		public event TitleChangedDelegate ProcessTitleChanged;
		Win32.WinEventDelegate processObjectNameChangeDelegate;

		void titleChanged(string title)
		{
			Console.WriteLine(title);
			Text = title;
			if (ProcessTitleChanged != null)
			{
				ProcessTitleChanged(this, title);
			}
			if (!readyTriggered && !string.IsNullOrEmpty(ReadyPattern) && ProcessReady != null)
			{
				if (readyPattern.IsMatch(title))
				{
					readyTriggered = true;
					ProcessReady(this);
				}
			}
		}

		void processObjectNameChange(IntPtr hWinEventHook, uint eventType,
			IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
		{
			try
			{
				if (hwnd == mainWindowHandle)
				{
					string title = ProcessTitle;
					if (title != Text)
					{
						titleChanged(title);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		private void processExited(object sender, EventArgs e)
		{
			Invoke(new MethodInvoker(processExitedInvoked));
		}

		private void processExitedInvoked()
		{
			releaseProcess();

			if ((AutoRestart || restartOnce) && !shutdownRequested)
				Launch();

			restartOnce = false;
			shutdownRequested = false;
		}

		public string ProcessTitle
		{
			get
			{
				if (mainWindowHandle != IntPtr.Zero)
				{
					int i = Win32.GetWindowTextLength(mainWindowHandle) + 1;
					StringBuilder sb = new StringBuilder(i);
					Win32.GetWindowText(mainWindowHandle, sb, i);
					return sb.ToString();
				}
				return LaunchCmd;
			}
		}

		uint mainWindowOriginalLong;
		Win32.RECT mainWindowOriginalRect;
		IntPtr mainWindowOriginalParent;

		void captureMainWindow()
		{
			mainWindowHandle = process.MainWindowHandle;
			titleChanged(ProcessTitle);

			mainWindowOriginalRect = new Win32.RECT();
			Win32.GetWindowRect(mainWindowHandle, ref mainWindowOriginalRect);

			mainWindowOriginalParent = Win32.GetParent(mainWindowHandle);
			Win32.SetParent(mainWindowHandle, Handle);

			mainWindowOriginalLong = Win32.GetWindowLong(mainWindowHandle, Win32.GWL_STYLE);
			Win32.SetWindowLong(mainWindowHandle, Win32.GWL_STYLE, mainWindowOriginalLong & ~Win32.WS_BORDER & ~Win32.WS_CAPTION & ~Win32.WS_THICKFRAME); // | Native.WS_CHILD

			Win32.ShowWindow(mainWindowHandle, Win32.SW_SHOWMAXIMIZED);
			Win32.SetWindowPos(mainWindowHandle, Handle, 0, 0, ClientSize.Width, ClientSize.Height, Win32.SWP_FRAMECHANGED | Win32.SWP_NOZORDER | Win32.SWP_SHOWWINDOW);
		}

		private void releaseMainWindow()
		{
			if (mainWindowHandle == IntPtr.Zero)
				return;

			if (processNameChangeHook != IntPtr.Zero)
			{
				Win32.UnhookWinEvent(processNameChangeHook);
				processNameChangeHook = IntPtr.Zero;
			}

			IntPtr handle = mainWindowHandle;
			mainWindowHandle = IntPtr.Zero;
			Win32.ShowWindow(handle, Win32.SW_SHOWNORMAL);
			Win32.SetParent(handle, mainWindowOriginalParent);
			Win32.SetWindowLong(handle, Win32.GWL_STYLE, mainWindowOriginalLong);
			Win32.SetWindowPos(handle, mainWindowOriginalParent, mainWindowOriginalRect.Left, mainWindowOriginalRect.Top, mainWindowOriginalRect.Right - mainWindowOriginalRect.Left, mainWindowOriginalRect.Bottom - mainWindowOriginalRect.Top, Win32.SWP_FRAMECHANGED | Win32.SWP_NOZORDER | Win32.SWP_SHOWWINDOW);
		}

		protected override void OnResize(EventArgs e)
		{
			// base.OnResize(e);
			if (mainWindowHandle == IntPtr.Zero)
				return;

			Win32.SetWindowPos(mainWindowHandle, Handle, 0, 0, ClientSize.Width, ClientSize.Height, Win32.SWP_NOZORDER | Win32.SWP_NOACTIVATE | Win32.SWP_NOMOVE);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed(e);
			releaseMainWindow();
		}
	}
}
