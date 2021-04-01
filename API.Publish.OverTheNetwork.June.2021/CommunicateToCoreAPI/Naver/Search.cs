using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

using ShareInvest.Client;

namespace ShareInvest.Naver
{
	public class Search
	{
		public async Task<Stack<string>> BrowseTheSite(string url)
		{
			try
			{
				var contents = new Stack<string>();
				var context = string.Empty;

				if (url.Split('.')[0][^4..] is string uri && cafe.Equals(uri) is false && post.Equals(uri) is false)
				{
					driver.Navigate().GoToUrl(url);
					var timeout = driver.Manage().Timeouts();
					timeout.ImplicitWait = TimeSpan.FromSeconds(0xC);
					timeout.PageLoad = TimeSpan.FromSeconds(0xC);
					await Task.Delay(0x200);
					driver.Manage().Window.FullScreen();

					switch (uri)
					{
						case blog:
							if (driver.FindElement(By.TagName(frame)) is IWebElement web)
							{
								var ds = driver.SwitchTo().Frame(web);

								foreach (var ct in ds.FindElements(By.Id($"post-view{url.Split('/')[^1]}")))
								{
									contents.Push(ds.Title.Split(':')[0].Replace("..", string.Empty).Trim());
									context = ct.Text;
								}
							}
							break;

						default:
							context = driver.PageSource;
							break;
					}
					if (string.IsNullOrEmpty(context) is false)
						foreach (var str in new Regex(@"[-“”‘’…‧~!@#$%^&*()_+|<>?:;{}\]→[.,·'""+=`/\n\r\t\v\s\b]").Split(new Regex("<[^>]+>", RegexOptions.IgnoreCase).Replace(new Regex("<(script|style)[^>]*>[\\s\\S]*?</\\1>").Replace(context, string.Empty), string.Empty).Replace("\r\n", string.Empty).Replace("nbsp", string.Empty)))
							if (string.IsNullOrWhiteSpace(str) is false && contents.Any(o => o.Equals(str)) is false && str.Length > 1 && Array.Exists(str.ToCharArray(), o => char.IsLetter(o)))
							{
								var remove = new Queue<char>();
								var regex = str.Replace("아주경제", string.Empty).Trim();

								foreach (var symbol in str.ToCharArray())
									if (char.IsLetterOrDigit(symbol) is false)
										remove.Enqueue(symbol);

								while (remove.TryDequeue(out char symbol))
									regex = regex.Replace(symbol.ToString(), string.Empty);

								foreach (Match match in new Regex("(은|는|이|가|에|께|의|을|를|와|과|습니|입니|님)").Matches(regex))
									switch (match.Value)
									{
										case "습니":
											if (match.Index == regex.Length - 3)
												regex = regex.Remove(match.Index - 1);

											break;

										case "입니":
											if (match.Index == regex.Length - 3)
												regex = regex.Remove(match.Index);

											break;

										default:
											if (match.Index == regex.Length - 1)
												regex = regex.Replace(match.Value, string.Empty);

											break;
									}
								if (regex.Length > 1)
									contents.Push(regex);
							}
					new Actions(driver).SendKeys(Keys.Escape).Perform();
				}
				if (contents.Count > 0)
					return contents;
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
			}
			return null;
		}
		public async Task<(List<Catalog.KRX.Cloud>, Dictionary<string, string>)> VisualizeTheResultsOfAnAnalysis(string keyword)
		{
			try
			{
				driver.Navigate().GoToUrl(security.Kinds);
				var timeout = driver.Manage().Timeouts();
				timeout.ImplicitWait = TimeSpan.FromSeconds(0xC);
				timeout.PageLoad = TimeSpan.FromSeconds(0xC);
				await Task.Delay(0x200);
				IAction action = new Actions(driver).SendKeys(Keys.ArrowDown).Build(), enter = new Actions(driver).SendKeys(Keys.Enter).Build(), up = new Actions(driver).SendKeys(Keys.ArrowUp).Build();
				var list = new List<Catalog.KRX.Cloud>();
				var dictionary = new Dictionary<string, string>();
				driver.Manage().Window.FullScreen();

				foreach (var key in driver.FindElementsByXPath(security.Click[^1]))
				{
					key.SendKeys(keyword);
					enter.Perform();
					await Task.Delay(0x200);
				}
				foreach (var select in driver.FindElementsByXPath(security.Click[^2]))
				{
					select.Click();

					foreach (var op in select.FindElements(By.TagName("option")))
						if (score.Equals(op.GetAttribute("value")))
						{
							enter.Perform();

							break;
						}
						else
							action.Perform();
				}
				for (int i = security.Click.Length - 2; i > 3; i--)
				{
					await Task.Delay(0xC00);
					Web = driver.FindElementByXPath(security.Click[i - 1]);
					Web.Click();
					up.Perform();
				}
				foreach (var input in Web.FindElements(By.TagName("input")))
				{
					await Task.Delay(0x200);
					var id = input.GetAttribute("id");

					if (Array.Exists(security.Click, o => o.Substring(9, 5).Equals(id)))
					{
						foreach (var label in Web.FindElements(By.TagName("label")))
							if (id.Equals(label.GetAttribute("for")))
							{
								label.Click();
								await Task.Delay(0x200);
							}
						if (security.Click[0].Substring(9, 5).Equals(id))
							break;
					}
				}
				foreach (var web in Web.FindElements(By.TagName("g")))
					foreach (var item in web.FindElements(By.TagName("text")))
						list.Add(new Catalog.KRX.Cloud
						{
							Text = item.Text,
							Style = item.GetAttribute("style"),
							Anchor = item.GetAttribute("text-anchor"),
							Transform = item.GetAttribute("transform")
						});
				foreach (var a in Web.FindElements(By.TagName("a")))
				{
					var href = a.GetAttribute("href");

					if (href.StartsWith(security.News) is false && dictionary.TryGetValue(news, out string detail) && dictionary.Remove(news))
						dictionary[href] = detail;

					else if (a.GetAttribute("class").Equals(news))
					{
						var title = a.FindElement(By.TagName("strong"));
						dictionary[news] = title.Text;
					}
				}
				foreach (var key in Web.FindElements(By.TagName("table")))
				{
					var temp = new Dictionary<string, string>();

					foreach (var tr in key.FindElements(By.TagName("tr")))
						foreach (var td in tr.FindElements(By.TagName("td")))
						{
							var id = td.GetAttribute("id");

							if (string.IsNullOrEmpty(id) is false)
								switch (id[0])
								{
									case '1':
										temp[score] = td.Text;
										break;

									case '3' when temp.TryGetValue(score, out string text) && temp.Remove(score):
										temp[text] = td.Text;
										break;
								}
						}
					foreach (var kv in temp)
						if (list.Find(o => o.Text.Equals(kv.Key)) is Catalog.KRX.Cloud cloud && list.Remove(cloud) && int.TryParse(kv.Value, out int frequency))
							list.Add(new Catalog.KRX.Cloud
							{
								Text = cloud.Text,
								Anchor = cloud.Anchor,
								Style = cloud.Style.Remove(cloud.Style.Length - 1, 1).Replace("\"", string.Empty),
								Transform = cloud.Transform,
								Frequency = frequency
							});
				}
				while ((driver as IJavaScriptExecutor).ExecuteScript(Base.Command) is not 0x64L and < 1e+2)
				{
					await Task.Delay(0x40);
					action.Perform();
				}
				new Actions(driver).SendKeys(Keys.Escape).Perform();

				if (list.Count > 0)
					return (list, dictionary);
			}
			catch (Exception ex)
			{
				Base.SendMessage(GetType(), keyword, ex.StackTrace);
			}
			finally
			{
				driver.Close();
				driver.Dispose();
				service.Dispose();
			}
			return (null, null);
		}
		public async Task<Dictionary<string, string>> SearchForKeyword(string param, int length)
		{
			try
			{
				var keywords = security.GoToKeyword(param);
				var response = new Dictionary<string, string>();

				for (int i = 0; i < keywords.Length; i++)
				{
					driver.Navigate().GoToUrl(keywords[i]);
					var timeout = driver.Manage().Timeouts();
					timeout.ImplicitWait = TimeSpan.FromSeconds(0xC);
					timeout.PageLoad = TimeSpan.FromSeconds(0xC);
					driver.Manage().Window.FullScreen();
					var action = new Actions(driver).SendKeys(Keys.ArrowDown).Build();

					switch (i)
					{
						case 1:
							var stack = new Stack<IWebElement>();

							while (length-- > 0)
							{
								await Task.Delay(0x100);
								action.Perform();

								if ((driver as IJavaScriptExecutor).ExecuteScript(Base.Command) is 0x64L or > 1e+2)
									break;
							}
							foreach (var x in driver.FindElementsByXPath(security.PathKeyword[i]))
								foreach (var ul in x.FindElements(By.TagName("li")))
									foreach (var div in ul.FindElements(By.TagName("div")))
									{
										var attribute = div.GetAttribute("data-cr-rank");

										if (string.IsNullOrEmpty(attribute) is false)
											stack.Push(div);
									}
							while (stack.TryPop(out IWebElement web))
							{
								var title = string.Empty;

								foreach (var a in web.FindElements(By.TagName("a")))
								{
									var confirm = a.GetAttribute("class");

									if (security.Title.Equals(confirm))
										title = a.Text.Trim();

									if (dsc.Equals(confirm) && string.IsNullOrEmpty(title) is false)
										response[a.GetAttribute("href")] = title;
								}
							}
							break;

						case 0:
							int page = driver.FindElementByXPath(security.PathKeyword[i]).FindElement(By.TagName("tbody")).FindElements(By.TagName("td")).Count, index = 0;

							if (driver.FindElementByXPath($"//*[@id='{security.PathKeyword[i + 6]}']").FindElements(By.TagName("div")).Count > 0)
								do
								{
									foreach (var div in driver.FindElementByXPath($"//*[@id='{security.PathKeyword[i + 3]}']").FindElements(By.TagName("div")))
										if (security.PathKeyword[i + 5].Equals(div.GetAttribute("class")))
										{
											var a = div.FindElement(By.TagName("a"));
											action.Perform();

											foreach (var title in a.FindElements(By.TagName("div")))
												if (security.PathKeyword[i + 4].Equals(title.GetAttribute("class")))
													response[a.GetAttribute("href")] = title.Text.Trim();
										}
									if (page > 0 && index < page - 3)
									{
										driver.FindElementByXPath($"//*[@id='{security.PathKeyword[i + 2]}']").Click();
										driver.Manage().Window.FullScreen();
									}
									await Task.Delay(0x100);
								}
								while (index++ < page - 3);

							break;
					}
					new Actions(driver).SendKeys(Keys.Escape).Perform();
				}
				if (response.Count > 0)
					return response;
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
			}
			return null;
		}
		public Search(dynamic key)
		{
			security = new Security(key);

			if (security.GrantAccess)
			{
				service = ChromeDriverService.CreateDefaultService(security.Path[0]);
				service.HideCommandPromptWindow = true;
				var options = new ChromeOptions();
				options.AddArgument($"--window-size=1015,{(Base.IsDebug ? 0x427 : 0x401)}");
				options.AddArgument(string.Concat("user-agent=", security.Path[^1]));
				driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(0x40));
			}
		}
		IWebElement Web
		{
			get; set;
		}
		const string news = "news-detail";
		const string dsc = "total_dsc";
		const string blog = "blog";
		const string cafe = "cafe";
		const string post = "post";
		const string score = "score";
		const string frame = "iframe";
		readonly Security security;
		readonly ChromeDriver driver;
		readonly ChromeDriverService service;
	}
}