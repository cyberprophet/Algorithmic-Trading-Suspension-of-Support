using System;

using OpenQA.Selenium.Chrome;

using ShareInvest.Client;

namespace ShareInvest.KRX
{
	public class Incorporate
	{
		public Incorporate(dynamic key)
		{
			var security = new Security(key);

			if (security.GrantAccess)
			{
				service = ChromeDriverService.CreateDefaultService(security.Path[0]);
				service.HideCommandPromptWindow = true;
				var options = new ChromeOptions();
				options.AddArgument("--window-size=631,493");
				options.AddArgument(string.Concat("user-agent=", security.Path[^1]));
				driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(0xA));
			}
		}
		readonly ChromeDriver driver;
		readonly ChromeDriverService service;
	}
}