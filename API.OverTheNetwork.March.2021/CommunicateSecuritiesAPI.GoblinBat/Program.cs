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

				if (Security.CheckAccessRights(args))
					StartProgress(args, new OpenAPI.ConnectAPI());
			}
			GC.Collect();
			Process.GetCurrentProcess().Kill();
		}
		static void StartProgress(dynamic param, Interface.ISecuritiesAPI<EventHandler.SendSecuritiesAPI> api)
		{
			if (api is OpenAPI.ConnectAPI)
				Application.Run(new SecuritiesAPI(param, api));

			if (Base.IsDebug == false)
			{
				Process.Start("shutdown.exe", "-r");
				Base.SendMessage(Security.Initialize(param).Item2);
			}
		}
	}
}