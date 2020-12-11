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
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new SecuritiesAPI(args));
			}
			GC.Collect();
			Process.GetCurrentProcess().Kill();
		}
	}
}