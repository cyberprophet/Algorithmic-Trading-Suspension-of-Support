using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

using RestSharp;

using ShareInvest.Client;

namespace ShareInvest.KRX
{
	public class Incorporate
	{
		public async Task<object> GetConstituentStocks(DateTime now, int page)
		{
			try
			{
				var market = page % 2 == 0 ? 'P' : 'Q';
				var request = new RestRequest(Security.RequestToKRX[2], Method.POST).AddHeader(Security.content_type, Security.RequestToKRX[3]);

				foreach (var kv in Security.GetParameter((int)bid, market, now))
					request.AddParameter(kv.Key, kv.Value);

				var response = await client.ExecuteAsync(request, source.Token);

				if (response.StatusCode.Equals(HttpStatusCode.OK))
					switch (bid)
					{
						case Interface.KRX.Catalog.지수구성종목:
							var stack = new Stack<Catalog.IncorporatedStocks>();

							foreach (var str in JsonConvert.DeserializeObject<List<Catalog.KRX.MDCSTAT00601>>(JObject.Parse(response.Content)[Security.RequestToKRX[^1]].ToString()))
								if (ulong.TryParse(str.Capitalization.Replace(",", string.Empty), out ulong capitalization))
									stack.Push(new Catalog.IncorporatedStocks
									{
										Code = str.Code,
										Name = str.Name,
										Date = (now.Hour < 9 ? now.AddDays(-1) : now).ToString(Base.DateFormat),
										Market = market,
										Capitalization = (int)(capitalization / 0x5F5E100)
									});
							return stack;
					}
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), ex.StackTrace);
			}
			return null;
		}
		public async Task<object> GetConstituentStocks(Interface.KRX.Catalog item, int page)
		{
			try
			{
				var search = Enum.GetName(typeof(JS), page % 2 == 0 ? 'P' : 'Q').Replace("_", " ");
				driver.Navigate().GoToUrl(Security.KRX);
				driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(7);
				var element = driver.FindElementByXPath($"//*[@id='{Enum.GetName(typeof(JS), 0)}']");

				foreach (var statistics in element.FindElements(By.TagName("ol")))
				{
					foreach (var move in statistics.FindElements(By.TagName("li")))
					{
						var name = move.FindElement(By.TagName("a"));

						if (name.GetAttribute("class").Equals(Enum.GetName(typeof(JS), 1)))
						{
							if (Enum.GetName(item).Equals(name.Text))
							{
								name.Click();

								switch (item)
								{
									case Interface.KRX.Catalog.지수구성종목:
										driver.FindElementByXPath($"//*[@id='{Enum.GetName(typeof(JS), 2)}']").SendKeys(search);
										new Actions(driver).SendKeys(Keys.Enter).Perform();
										await Task.Delay(0x400);
										new Actions(driver).SendKeys(Keys.Enter).Perform();

										foreach (var td in driver.FindElementByXPath($"//*[@id='{Enum.GetName(typeof(JS), 5)}']").FindElements(By.TagName("td")))
											if (search.Equals(td.Text))
											{
												td.Click();
												var choice = driver.FindElementByXPath($"//*[@id='{Enum.GetName(typeof(JS), 6)}']").FindElement(By.ClassName(Enum.GetName(typeof(JS), 7).Replace("_", "-")));
												choice.Click();

												foreach (var option in choice.FindElements(By.TagName("option")))
													if ("1".Equals(option.GetAttribute("value")))
														option.Click();

												break;
											}
										var stack = new Stack<Catalog.IncorporatedStocks>();
										var command = driver.FindElementByXPath($"//*[@id='{Enum.GetName(typeof(JS), 3)}']");
										command.Click();
										await Task.Delay(0xC00);
										driver.Manage().Window.FullScreen();
										driver.FindElementByXPath($"//*[@id='{Enum.GetName(typeof(JS), 0xA)}']").Click();

										return stack;
								}
							}
							else
								continue;
						}
						else
						{
							new Actions(driver).MoveToElement(element.FindElement(By.ClassName("on"))).Perform();
							await Task.Delay(0x400);
							new Actions(driver).MoveToElement(statistics.FindElement(By.TagName("a"))).Perform();
						}
						new Actions(driver).MoveToElement(name).Perform();
					}
					break;
				}
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
			return null;
		}
		public Incorporate(dynamic key)
		{
			var security = new Security(key);

			if (security.GrantAccess)
			{
				service = ChromeDriverService.CreateDefaultService(security.Path[0]);
				service.HideCommandPromptWindow = true;
				var options = new ChromeOptions();
				options.AddArgument("--window-size=1536,992");
				options.AddArgument(string.Concat("user-agent=", security.Path[^1]));
				driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(9));
			}
		}
		public Incorporate(Interface.KRX.Catalog bid, dynamic key)
		{
			var security = new Security(key);

			if (security.GrantAccess)
			{
				client = new RestClient(Security.RequestToKRX[0])
				{
					Timeout = -1,
					UserAgent = Security.RequestToKRX[1]
				};
				source = new CancellationTokenSource();
			}
			this.bid = bid;
		}
		readonly Interface.KRX.Catalog bid;
		readonly IRestClient client;
		readonly CancellationTokenSource source;
		readonly ChromeDriver driver;
		readonly ChromeDriverService service;
	}
}