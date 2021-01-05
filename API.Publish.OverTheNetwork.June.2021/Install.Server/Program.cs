using System;
using System.Diagnostics;

namespace ShareInvest
{
	class Program
	{
		static void Main()
		{			
			if (Security.DirectoryInfo && Firewall.AddInboudRule(Firewall.Name, Protocol.Tcp, Firewall.Port) && Security.UpdateToVersion(DateTime.Now.AddDays(-1)) && Security.UpdateToVersion())
				StartProgress();

			else
				Process.Start(Security.StartInfo);
		}
		static void StartProgress()
		{
			new Security(Verify.KeyDecoder.ProductKeyFromRegistry).StartProgress();
			Process.GetCurrentProcess().Kill();
		}
	}
}