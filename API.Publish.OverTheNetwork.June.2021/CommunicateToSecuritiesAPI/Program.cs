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

				if (Security.GetAdministrator(args) is string str)
					StartProgress(str, new OpenAPI.ConnectAPI());
			}
			GC.Collect();
			Process.GetCurrentProcess().Kill();
		}
		static void StartProgress(dynamic param, Interface.ISecuritiesAPI<EventHandler.SendSecuritiesAPI> api)
		{
			if (api is OpenAPI.ConnectAPI)
				Application.Run(new SecuritiesAPI(param, api));
		}
	}
}