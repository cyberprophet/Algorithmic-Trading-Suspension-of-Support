using System;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

using ShareInvest.Client;

namespace ShareInvest.Naver
{
	public class Search
	{
		public async Task SearchForKeyword(string param, int length)
		{
			try
			{
				driver.Navigate().GoToUrl($"https://search.naver.com/search.naver?where=view&query={param}&sm=tab_viw.all&nso=so%3Ar%2Cp%3A1d%2Ca%3Aall&mode=timeline&main_q=&st_coll=&topic_r_cat=");
				driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0xC);
				driver.Manage().Window.FullScreen();
				var action = new Actions(driver).SendKeys(Keys.ArrowDown).Build();

				while (length-- > 0)
				{
					await Task.Delay(0x100);
					action.Perform();

					if ((driver as IJavaScriptExecutor).ExecuteScript("return (window.scrollY + window.innerHeight) / document.body.clientHeight * 100") is 0x64L or > 1e+2)
						break;
				}
				new Actions(driver).SendKeys(Keys.Escape).Perform();
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
		public Search(dynamic key)
		{
			var security = new Security(key);

			if (security.GrantAccess)
			{
				service = ChromeDriverService.CreateDefaultService(security.Path[0]);
				service.HideCommandPromptWindow = true;
				var options = new ChromeOptions();
				options.AddArgument($"--window-size=1015,{(Base.IsDebug ? 0x427 : 0x401)}");
				options.AddArgument(string.Concat("user-agent=", security.Path[^1]));
				driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(0x21));
			}
		}
		readonly ChromeDriver driver;
		readonly ChromeDriverService service;
	}
}