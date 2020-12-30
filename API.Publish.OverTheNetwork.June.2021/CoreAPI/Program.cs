using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShareInvest
{
	static class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			if (Application.SetHighDpiMode(HighDpiMode.SystemAware))
			{
				var security = new Security(args);

				if (Progress.GetUpdateVisionAsync().Result)
					Process.Start("shutdown.exe", "-r");

				else
				{
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);

					if (security.CheckAccessRights)
						Application.Run(new CoreAPI(security));
				}
			}
			GC.Collect();
			Process.GetCurrentProcess().Kill();
		}
	}
}