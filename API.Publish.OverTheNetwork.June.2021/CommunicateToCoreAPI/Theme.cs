using System;
using System.Collections.Generic;
using System.Linq;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ShareInvest.Client
{
	public sealed class Theme
	{
		public (uint, IEnumerable<Catalog.Models.Theme>) OnReceiveMarketPriceByTheme(int page)
		{
			try
			{
				driver.Navigate().GoToUrl(Security.RequestTheIntegratedAddress(this, page));
				driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
				var queue = new Queue<Catalog.Models.Theme>();
				var last_page = 0;

				foreach (var context in driver.FindElementsByXPath(Security.RequestMarketPriceByTheme(2)))
					foreach (var td in context.FindElements(By.TagName("td")))
						if (int.TryParse(td.FindElement(By.TagName("a")).GetAttribute("href").Split('=')[^1], out int num))
							last_page = num;

				if (page < last_page + 1)
				{
					foreach (var context in driver.FindElementsByXPath(Security.RequestMarketPriceByTheme(1)))
						foreach (var tr in context.FindElements(By.TagName("tr")))
						{
							var theme = new Catalog.Models.Theme
							{
								Code = new string[2]
							};
							foreach (var td in tr.FindElements(By.TagName("td")))
								if (Enum.TryParse(td.GetAttribute("class").Replace("number ", string.Empty).Replace("ls ", string.Empty), out ThemeType type))
									switch ((int)type)
									{
										case 1:
											theme.Name = td.Text.Trim();
											theme.Index = td.FindElement(By.TagName("a")).GetAttribute("href").Split('=')[^1];
											break;

										case 2:
										case 3:
											if (double.TryParse(td.Text.Split('%')[0], out double rate))
												switch (type)
												{
													case ThemeType.col_type2:
														theme.Rate = rate;
														break;

													case ThemeType.col_type3:
														theme.Average = rate;
														break;
												}
											continue;

										case 5:
										case 6:
											if (td.FindElement(By.TagName("a")).GetAttribute("href").Split('=')[^1] is string code && code.Length == 6)
												switch (type)
												{
													case ThemeType.col_type5:
														theme.Code[0] = code;
														break;

													case ThemeType.col_type6:
														theme.Code[1] = code;
														break;
												}
											continue;

										default:
											continue;
									}
							if (string.IsNullOrEmpty(theme.Name) is false)
								queue.Enqueue(theme);
						}
					return ((uint)(1 + last_page), queue.OrderBy(o => Guid.NewGuid()));
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
			return (uint.MinValue, null);
		}
		public Queue<Catalog.Models.GroupDetail> GetDetailsFromGroup(string page, int index)
		{
			try
			{
				driver.Navigate().GoToUrl(Security.RequestTheIntegratedAddress(this, page));
				driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0x11);
				var group = new Queue<Catalog.Models.GroupDetail>();

				foreach (var context in driver.FindElementsByXPath(Security.RequestDetailsFromGroup(index)))
					foreach (var tbody in context.FindElements(By.TagName("tbody")))
					{
						var count = 0;
						var temp = new string[0xA];

						foreach (var td in tbody.FindElements(By.TagName("td")))
						{
							var attribute = td.GetAttribute("class");

							if (Enum.TryParse(attribute, out GroupDetailType type))
								switch (type)
								{
									case GroupDetailType.name:
										continue;

									case GroupDetailType.number:
										temp[count++] = td.Text.Replace(",", string.Empty);
										break;

									case GroupDetailType.center:
										if (td.FindElement(By.TagName("a")).GetAttribute("href").Split('=')[^1] is string code && code.Length == 6)
										{
											if (int.TryParse(temp[8], out int before) && int.TryParse(temp[6], out int volume) && double.TryParse(temp[3].Replace("%", string.Empty), out double rate) && int.TryParse(temp[1], out int current))
												group.Enqueue(new Catalog.Models.GroupDetail
												{
													Code = code,
													Index = page,
													Title = temp[0],
													Current = current,
													Rate = rate * 1e-2,
													Degree = double.NaN,
													Percent = double.NaN,
													Compare = volume / (double)before
												});
											temp[count] = code;
										}
										break;
								}
							else if (string.IsNullOrEmpty(attribute))
								foreach (var p in td.FindElements(By.TagName("p")))
								{
									count = 0;
									temp[count++] = p.GetAttribute("textContent");
								}
						}
					}
				if (group.Count > 0)
					return group;
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), page, ex.StackTrace);
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
		public Theme(dynamic key)
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