using System;
using System.Collections.Generic;
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

				if (url.Split('.')[0][^4..] is string uri && cafe.Equals(uri) is false && post.Equals(uri) is false && Enum.TryParse(url.Split('.')[1], contents.Count == 0, out Interface.KRX.News news) && Interface.KRX.News.SEdaily.Equals(news) is false && Interface.KRX.News.MT.Equals(news) is false)
				{
					driver.Navigate().GoToUrl(url);
					var timeout = driver.Manage().Timeouts();
					timeout.ImplicitWait = TimeSpan.FromSeconds(0xC);
					timeout.PageLoad = TimeSpan.FromSeconds(0xC);
					await Task.Delay(0x200);
					driver.Manage().Window.FullScreen();

					switch (news)
					{
						case Interface.KRX.News.Naver:
							if (blog.Equals(uri) && driver.FindElement(By.TagName(frame)) is IWebElement web)
							{
								var ds = driver.SwitchTo().Frame(web);

								foreach (var ct in ds.FindElements(By.Id($"post-view{url.Split('/')[^1]}")))
								{
									var title = ds.Title.Split(':')[0].Replace("..", string.Empty).Trim();
									contents.Push(title);
									contents.Push(ct.Text);

									if (Base.IsDebug)
										Base.SendMessage(news.GetType(), title, ct.Text, news);
								}
							}
							break;

						case Interface.KRX.News.AjuNews:
							foreach (var path in driver.FindElementsByXPath($"//*[@id='{security.PathToContents[^1]}']"))
								if (string.IsNullOrEmpty(path.Text) is false)
								{
									contents.Push(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(path.Text, "(\\(|\\[)사진[^)\\]]*(\\)|])", string.Empty, RegexOptions.Singleline), "(\\(|\\[)그래픽[^)\\]]*(\\)|])", string.Empty, RegexOptions.Singleline), "(\\(|\\[)자료[^)\\]]*(\\)|])", string.Empty, RegexOptions.Singleline), "(\\(|\\[)이미지[^)\\]]*(\\)|])", string.Empty, RegexOptions.Singleline));

									if (Base.IsDebug)
										Base.SendMessage(news.GetType(), path.Text, news);
								}
								else
									foreach (var div in path.FindElements(By.TagName("div")))
										if (string.IsNullOrEmpty(div.Text) is false)
										{
											contents.Push(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(div.Text, "(\\(|\\[)사진[^)\\]]*(\\)|])", string.Empty, RegexOptions.Singleline), "(\\(|\\[)그래픽[^)\\]]*(\\)|])", string.Empty, RegexOptions.Singleline), "(\\(|\\[)자료[^)\\]]*(\\)|])", string.Empty, RegexOptions.Singleline), "(\\(|\\[)이미지[^)\\]]*(\\)|])", string.Empty, RegexOptions.Singleline));

											if (Base.IsDebug)
												Base.SendMessage(news.GetType(), div.Text, news);
										}
							break;

						case Interface.KRX.News.MK:
							if (uri[1..].Equals("vip"))
							{
								foreach (var table in driver.FindElementByXPath($"//*[@id='{security.PathToContents[^2]}']").FindElements(By.TagName("table")))
									if (security.PathToContents[^3].Equals(table.GetAttribute("class")))
										if (string.IsNullOrEmpty(table.Text) is false)
										{
											contents.Push(table.Text);

											if (Base.IsDebug)
												Base.SendMessage(news.GetType(), table.Text, news);
										}
							}
							else
								foreach (var div in driver.FindElementsByXPath($"//*[@id='{security.PathToContents[^2]}']"))
								{
									if (string.IsNullOrEmpty(div.Text) is false)
										contents.Push(div.Text);

									foreach (var p in div.FindElements(By.TagName("p")))
										if (string.IsNullOrEmpty(p.Text) is false)
										{
											contents.Push(p.Text);

											if (Base.IsDebug)
												Base.SendMessage(news.GetType(), p.Text, news);
										}
									if (Base.IsDebug)
										Base.SendMessage(news.GetType(), div.Text, news);
								}
							break;

						case Interface.KRX.News.TheKPM:
							foreach (var div in driver.FindElements(By.TagName("div")))
								if (security.PathToContents[^1].Equals(div.GetAttribute(security.PathToContents[^5])) && string.IsNullOrEmpty(div.Text) is false)
								{
									var text = Regex.Replace(div.Text, "(<|\\[)[^>\\]]*(>|])", string.Empty, RegexOptions.Singleline);
									contents.Push(text);

									if (Base.IsDebug)
										Base.SendMessage(news.GetType(), text, news);
								}
							break;

						case Interface.KRX.News.EconoNews:
							foreach (var p in driver.FindElementByXPath($"//*[@id='{security.PathToContents[^1]}']").FindElements(By.TagName("p")))
								if (string.IsNullOrEmpty(p.Text) is false)
								{
									contents.Push(p.Text);

									if (Base.IsDebug)
										Base.SendMessage(news.GetType(), p.Text, news);
								}
							break;

						case Interface.KRX.News.Asiae:
							foreach (var div in driver.FindElementByXPath($"//*[@id='{security.PathToContents[^6]}']").FindElements(By.TagName("div")))
								if (security.PathToContents[^1].Equals(div.GetAttribute(security.PathToContents[^5])))
									foreach (var p in div.FindElements(By.TagName("p")))
										if (string.IsNullOrEmpty(p.Text) is false)
										{
											contents.Push(p.Text);

											if (Base.IsDebug)
												Base.SendMessage(news.GetType(), p.Text, news);
										}
							break;

						case Interface.KRX.News.WOWTV:
							var wow = driver.FindElementByXPath($"//*[@id='{security.PathToContents[^7]}']").Text;

							if (string.IsNullOrEmpty(wow) is false)
							{
								contents.Push(wow);

								if (Base.IsDebug)
									Base.SendMessage(news.GetType(), wow, news);
							}
							break;

						case Interface.KRX.News.MTN:
							var mtn = driver.FindElementByXPath($"//*[@id='{security.PathToContents[^4]}']").Text;

							if (string.IsNullOrEmpty(mtn) is false)
							{
								contents.Push(mtn);

								if (Base.IsDebug)
									Base.SendMessage(news.GetType(), mtn, news);
							}
							break;

						case Interface.KRX.News.Edaily:
							foreach (var section in driver.FindElementByXPath($"//*[@id='{security.PathToContents[^8]}']").FindElements(By.TagName("section")))
								if (security.PathToContents[^9].Equals(section.GetAttribute("class")))
									foreach (var div in section.FindElements(By.TagName("div")))
										if (security.PathToContents[^1].Equals(div.GetAttribute(security.PathToContents[^5])) && string.IsNullOrEmpty(div.Text) is false)
										{
											contents.Push(div.Text);

											if (Base.IsDebug)
												Base.SendMessage(news.GetType(), div.Text, news);
										}
							break;

						case Interface.KRX.News.EToday:
							foreach (var div in driver.FindElementByTagName("body").FindElements(By.TagName("div")))
								if ("wrap".Equals(div.GetAttribute("class")))
									foreach (var article in div.FindElements(By.TagName("article")))
										if ("containerWrap".Equals(article.GetAttribute("class")))
											foreach (var section in article.FindElements(By.TagName("div")))
												if ("view_body_moduleWrap".Equals(section.GetAttribute("class")))
													foreach (var item in section.FindElements(By.TagName("div")))
														if (security.PathToContents[^1].Equals(item.GetAttribute(security.PathToContents[^5])))
															foreach (var p in item.FindElements(By.TagName("p")))
																if (string.IsNullOrEmpty(p.Text) is false)
																{
																	contents.Push(p.Text);

																	if (Base.IsDebug)
																		Base.SendMessage(news.GetType(), p.Text, news);
																}
							break;

						case Interface.KRX.News.NewsPim:
							if (uri[^1] is 'm' && url.Split('.') is string[] www)
							{
								driver.Navigate().GoToUrl(string.Concat(www[0].Replace("m", nameof(www)), '.', www[1], '.', www[^1]));
								timeout = driver.Manage().Timeouts();
								timeout.ImplicitWait = TimeSpan.FromSeconds(0xC);
								timeout.PageLoad = TimeSpan.FromSeconds(0xC);
								await Task.Delay(0x200);
							}
							foreach (var p in driver.FindElementByXPath($"//*[@id='{security.PathToContents[^0xA]}']").FindElements(By.TagName("p")))
								if (string.IsNullOrEmpty(p.Text) is false)
								{
									contents.Push(p.Text);

									if (Base.IsDebug)
										Base.SendMessage(news.GetType(), p.Text, news);
								}
							break;

						case Interface.KRX.News.NewsTomato:
							foreach (var rn_container in driver.FindElementByXPath($"//*[@id='{security.PathToContents[^0xB]}']").FindElements(By.TagName("div")))
								if (nameof(rn_container).Equals(rn_container.GetAttribute("class")))
									foreach (var article in rn_container.FindElements(By.TagName("article")))
										foreach (var rn_scontent in article.FindElements(By.TagName("div")))
											if (nameof(rn_scontent).Equals(rn_scontent.GetAttribute("class")))
												foreach (var section in rn_scontent.FindElements(By.TagName("section")))
													foreach (var rns_text in section.FindElements(By.TagName("div")))
														if (nameof(rns_text).Equals(rns_text.GetAttribute("class")) && string.IsNullOrEmpty(rns_text.Text) is false)
														{
															contents.Push(rns_text.Text);

															if (Base.IsDebug)
																Base.SendMessage(news.GetType(), rns_text.Text, news);
														}
							break;
					}
					new Actions(driver).SendKeys(Keys.Escape).Perform();

					if (contents.Count > 0)
						return contents;
				}
				else if (Base.IsDebug)
					Base.SendMessage(GetType(), url);
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