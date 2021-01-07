using System;
using System.Diagnostics;

namespace ShareInvest
{
	class Program
	{
		static void Main()
		{
			var security = new Security(Verify.KeyDecoder.ProductKeyFromRegistry);

			if (security.IsProgress)
				security.StartProgress();

			else
			{
				if (Security.DirectoryInfo && Firewall.AddInboudRule(Firewall.Name, Protocol.Tcp, Firewall.Port) && Security.UpdateToVersion(DateTime.Now.AddDays(-1)) && Security.UpdateToVersion())
					security.StartProgress();

				else
					Process.Start(Security.StartInfo);
			}
		}
	}
}