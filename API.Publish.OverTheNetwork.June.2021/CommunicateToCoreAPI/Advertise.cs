using System;
using System.Threading.Tasks;

using OpenQA.Selenium.Chrome;

namespace ShareInvest.Client
{
	public class Advertise
	{
		public async Task StartAdvertisingInTheDataCollectionSection(int count)
		{
			try
			{
				string url;
				var page = count % 0x33;

				if (page is 0xF or 0x10 or 0x11 or 0x12 or < 7)
					url = tistory.Remove(tistory.Length - 1, 1);

				else
					url = string.Concat(tistory, page);

				driver.Navigate().GoToUrl(url);
				driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0xA);
				await Task.Delay(0x2800);
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
			}
			finally
			{
				driver.Close();
				driver.Dispose();
				service.Dispose();
				GC.Collect();
			}
		}
		public Advertise(dynamic key)
		{
			var security = new Security(key);
			service = ChromeDriverService.CreateDefaultService(security.Path[0]);
			service.HideCommandPromptWindow = true;
			var options = new ChromeOptions();
			options.AddArgument("--window-size=818,673");
			options.AddArgument(string.Concat("user-agent=", security.Path[^1]));
			driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(0x21));
		}
		readonly ChromeDriver driver;
		readonly ChromeDriverService service;
		const string tistory = @"https://sharecompany.tistory.com/";
	}
}