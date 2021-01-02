using System;
using System.Diagnostics;

namespace ShareInvest
{
	class Program
	{
		static void Main()
		{
			if (Security.UpdateToVersion())
				StartProgress();

			else
				Process.Start(Security.StartInfo);
		}
		static void StartProgress()
		{
			new Security(Verify.KeyDecoder.ProductKeyFromRegistry).StartProgress();
			GC.Collect();
			Process.GetCurrentProcess().Kill();
		}
	}
}