using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Win32;

namespace ShareInvest
{
	sealed partial class Install : Form
	{
		internal Install(string key)
		{
			InitializeComponent();
			this.key = GetUserInformation(key);
			color = new Color[] { Color.Gold, Color.Ivory, Color.DeepSkyBlue };
			timer.Start();
		}
		(bool, DirectoryInfo) IsExist(string path)
		{
			var di = new DirectoryInfo(path);

			return (di.Exists, di);
		}
		bool StartProgress(Tuple<string, string, string> process) => Process.GetProcessesByName(process.Item1.Split('.')[0]).Length == 0 && new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = process.Item1,
				Arguments = process.Item2,
				UseShellExecute = true,
				WorkingDirectory = process.Item3
			}
		}.Start();
		void WorkerProgressChanged(object sender, ProgressChangedEventArgs e) => progress.Value += progress.Value < 0x64 ? e.ProgressPercentage : 0;
		void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error is Exception)
			{

			}
			else if (e.Cancelled)
				Dispose();

			else if (e.Result is int result)
				progress.Value = result;
		}
		void WorkerDoWork(object sender, DoWorkEventArgs e)
		{
			string update = Path.Combine(path[0], @"Res\Update"), z64 = "x64.zip", z86 = "x86.zip";
			worker.ReportProgress(1);

			for (int i = 0; i < path.Length - 2; i++)
			{
				var path = IsExist(this.path[i]);

				if (path.Item1 is false)
				{
					path.Item2.Create();
					worker.ReportProgress(1);
				}
			}
			if (path[0].Equals(Application.StartupPath))
			{
				foreach (var index in new[] { 3, 4 })
					if (IsExist(path[index]).Item1 is false)
					{
						worker.ReportProgress(1);
					}
				var now = DateTime.Now.AddDays(-1);
				worker.ReportProgress(1);
				FileInfo x64 = new FileInfo(Path.Combine(update, z64)), x86 = new FileInfo(Path.Combine(update, z86));
				int n64 = now.CompareTo(x64.LastWriteTime), n86 = now.CompareTo(x86.LastWriteTime);

				if (n64 < 0 || new DirectoryInfo(path[1]).GetFiles("*.exe", SearchOption.TopDirectoryOnly).Length == 0)
				{
					worker.ReportProgress(1);
					UpdateToVersion(path[3], Format(x64.FullName, path[1]), path[1]);
				}
				if (n86 < 0 || new DirectoryInfo(path[2]).GetFiles("*.exe", SearchOption.TopDirectoryOnly).Length == 0)
				{
					worker.ReportProgress(1);
					UpdateToVersion(path[3], Format(x86.FullName, path[2]), path[2]);
				}
			}
			else
				try
				{
					var exe = Application.ExecutablePath.Split('\\');
					File.Copy(Application.ExecutablePath, Path.Combine(this.path[0], exe[exe.Length - 1]));
					var path = new DirectoryInfo(update);

					if (path.Exists is false)
					{
						path.Create();
						worker.ReportProgress(1);
					}
					File.WriteAllBytes(Path.Combine(path.FullName, z64), Properties.Resources.x64);
					File.WriteAllBytes(Path.Combine(path.FullName, z86), Properties.Resources.x86);
					File.WriteAllBytes(Path.Combine(this.path[0], "chromedriver.exe"), Properties.Resources.chromedriver);
					timer.Stop();
					worker.ReportProgress(1);
					FirstUpdate = true;
					StartProgress(exe[exe.Length - 1]);
				}
				catch
				{
					e.Cancel = true;
				}
			e.Result = 0x64;
		}
		void StartProgress(string exe) => BeginInvoke(new Action(async () =>
		{
			if (new Process
			{
				StartInfo = new ProcessStartInfo
				{
					UseShellExecute = true,
					FileName = exe,
					WorkingDirectory = path[0],
					Verb = "runas"
				}
			}.Start())
			{
				await Task.Delay(0x3E9);
				timer.Start();
			}
		}));
		void TimerTick(object sender, EventArgs e)
		{
			if (progress.Value < 0x63)
			{
				if (progress.Value++ == 0)
					worker.RunWorkerAsync();

				label_name.ForeColor = color[DateTime.Now.Second % 3];
			}
			else if (progress.Value == 0x64)
			{
				timer.Stop();

				if (path[0].Equals(Application.StartupPath))
				{
					foreach (var name in name)
						using (var registry = Registry.CurrentUser.OpenSubKey(run))
							if (registry.GetValue(name) != null)
							{
								registry.Close();
								Registry.CurrentUser.OpenSubKey(run, true).DeleteValue(name);
							}
					using (var registry = Registry.CurrentUser.OpenSubKey(run))
						if (registry.GetValue(name[0]) is null)
						{
							registry.Close();
							Registry.CurrentUser.OpenSubKey(run, true).SetValue(name[0], Path.Combine(path[0], string.Concat(name[0], ".exe")));
						}
					if (StartProgress(new Tuple<string, string, string>(execute[0], key, path[1])))
					{
						if (FirstUpdate)
							foreach (var file in new DirectoryInfo(update).GetFiles("*.zip", SearchOption.TopDirectoryOnly))
								file.Delete();

						if (StartProgress(new Tuple<string, string, string>(execute[1], key, path[2])))
							Dispose();
					}
				}
				else
					Dispose();
			}
		}
		void UpdateToVersion(string directory, string command, string path)
		{
			using (var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "cmd",
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardInput = true,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					WorkingDirectory = directory
				}
			})
				if (process.Start())
				{
					process.StandardInput.Write(command + Environment.NewLine);
					process.StandardInput.Close();
					Console.WriteLine(process.StandardOutput.ReadToEnd());
					process.WaitForExit();
				}
			if (string.IsNullOrEmpty(path) is false)
				foreach (var file in Directory.GetFiles(path, "*.pdb", SearchOption.AllDirectories))
					File.Delete(file);
		}
		bool FirstUpdate
		{
			get; set;
		}
		readonly Color[] color;
	}
}