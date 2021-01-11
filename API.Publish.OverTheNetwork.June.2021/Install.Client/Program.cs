using System;
using System.Diagnostics;
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

			if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
				Application.Run(new Install(Verify.KeyDecoder.ProductKeyFromRegistry));

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
	}
}