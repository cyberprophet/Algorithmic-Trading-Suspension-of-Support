using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;

namespace ShareInvest
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			ChangePropertyToDebugMode();

			if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator) || ToDebug)
			{
				foreach (var file in Files)
					if (new FileInfo(Path.Combine(file.Item1, file.Item2)).Exists is false)
					{
						Process.Start(new ProcessStartInfo(file.Item3)
						{
							UseShellExecute = true
						});
						if (DialogResult.OK.Equals(MessageBox.Show("An essential element for program operation is missing.\n\nDownload from the link you are connecting to,\ninstall it in the captioned location\nat the top of the message box,\nand click the 'OK' button.\n\nIf you move on to the next step\nbefore the installation is finished,\nthe program will be terminated.", file.Item1, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)) && new FileInfo(Path.Combine(file.Item1, file.Item2)).Exists)
							continue;

						GC.Collect();
						Process.GetCurrentProcess().Kill();
					}
				var info = new DirectoryInfo(directory);

				if (info.Exists is false)
				{
					info.Create();
					File.WriteAllBytes(Path.Combine(directory, initialize), Properties.Resources.initialize);
				}
				foreach (var package in Packages)
					using (var process = new Process
					{
						StartInfo = new ProcessStartInfo
						{
							FileName = cmd,
							Verb = "runas",
							CreateNoWindow = true,
							UseShellExecute = false,
							RedirectStandardError = true,
							RedirectStandardInput = true,
							RedirectStandardOutput = true,
							WorkingDirectory = directory
						}
					})
						if (process.Start())
						{
							process.StandardInput.WriteLine(string.Concat(@"C:\R\R-4.0.4\bin\x64\rscript ", initialize, ' ', package));
							process.StandardInput.Close();
							Console.WriteLine(process.StandardOutput.ReadToEnd());
							process.WaitForExit();
						}
				Application.Run(new Install(Verify.KeyDecoder.ProductKeyFromRegistry));
			}
			else
			{
				var file = Application.ExecutablePath.Split('\\');

				if (new Process
				{
					StartInfo = new ProcessStartInfo
					{
						UseShellExecute = true,
						FileName = file[file.Length - 1],
						WorkingDirectory = Application.StartupPath,
						Verb = "runas"
					}
				}.Start())
					GC.Collect();
			}
		}
		[Conditional("DEBUG")]
		static void ChangePropertyToDebugMode() => ToDebug = true;
		static bool ToDebug
		{
			get; set;
		}
		static string[] Packages => new[] { "multilinguer", "hash", "tau", "Sejong", "RSQLite", "devtools", "bit", "rex", "lazyeval", "htmlwidgets", "crosstalk", "promises", "later", "sessioninfo", "xopen", "bit64", "blob", "DBI", "memoise", "plogr", "covr", "DT", "rcmdcheck", "rversions", "wordcloud", "RColorBrewer", "remotes" };
		static (string, string, string)[] Files => new[] { (@"C:\OpenAPI", "opstarter.exe", @"https://www2.kiwoom.com/nkw.templateFrameSet.do?m=m1408000000"), (@"C:\Program Files (x86)\ESTsoft\ALZip", "ALZipCon.exe", @"https://www.altools.co.kr/download/alzip.aspx"), (@"C:\R\R-4.0.4", @"bin\x64\Rscript.exe", @"https://cran.r-project.org/bin/windows/base"), (@"C:\rtools40", @"usr\bin\make.exe", @"https://cran.r-project.org/bin/windows/Rtools") };
		const string cmd = "cmd";
		const string initialize = "initialize.R";
		const string directory = @"C:\Algorithmic Trading\Res\R";
	}
}