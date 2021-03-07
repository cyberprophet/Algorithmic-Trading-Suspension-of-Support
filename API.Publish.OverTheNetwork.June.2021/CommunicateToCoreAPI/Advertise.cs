using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

using RestSharp;

namespace ShareInvest.Client
{
	public class Advertise
	{
		public async Task StartAdvertisingInTheDataCollectionSection(int page)
		{
			try
			{
				driver.Navigate().GoToUrl(string.Concat(tistory, page));
				var timeout = driver.Manage().Timeouts();
				timeout.ImplicitWait = TimeSpan.FromSeconds(0xC);
				timeout.PageLoad = TimeSpan.FromSeconds(0xC);
				await Task.Delay(0x200);
				var action = new Actions(driver).SendKeys(Keys.ArrowDown).Build();
				driver.Manage().Window.FullScreen();

				for (int i = page; i > 0; i--)
					if (page < 0x100)
					{
						action.Perform();
						await Task.Delay(i % 0x100 > 0x20 ? i % 0x100 : 0x40);
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
			}
		}
		public async Task<object> TransmitCollectedInformation(Catalog.Models.Codes cn, List<Catalog.KRX.Cloud> cloud, Dictionary<string, string> news)
		{
			try
			{
				driver.Navigate().GoToUrl(System.IO.Path.Combine(tistory, 0x32.ToString()));
				var timeout = driver.Manage().Timeouts();
				timeout.ImplicitWait = TimeSpan.FromSeconds(0xC);
				timeout.PageLoad = TimeSpan.FromSeconds(0xC);
				await Task.Delay(0x200);
				driver.Manage().Window.FullScreen();

				foreach (var header in driver.FindElementsByXPath(security.Transmit[^1]))
					foreach (var button in header.FindElements(By.TagName("button")))
					{
						var title = button.GetAttribute("title");

						if (string.IsNullOrEmpty(title) is false && menu.Equals(title))
						{
							button.Click();
							await Task.Delay(0x400);
						}
					}
				foreach (var aside in driver.FindElementsByXPath(security.Transmit[^2]))
					foreach (var div in aside.FindElements(By.TagName(nameof(aside))))
						foreach (var btn in div.FindElements(By.TagName(nameof(div))))
						{
							var attribute = btn.GetAttribute("class");

							if (string.IsNullOrEmpty(attribute) is false && attribute.StartsWith("btn-for-") && attribute.EndsWith("guest"))
							{
								btn.FindElement(By.TagName("a")).Click();
								await Task.Delay(0x400);

								foreach (var main in driver.FindElementsByXPath(security.Transmit[^3]))
									foreach (var a in main.FindElements(By.TagName("a")))
									{
										var login = a.GetAttribute("class");

										if (string.IsNullOrEmpty(login) is false && login.EndsWith(security.Transmit[^4]))
										{
											a.Click();
											await Task.Delay(0x400);
											driver.FindElementByXPath(security.Transmit[^5]).SendKeys(security.Transmit[^6]);
											await Task.Delay(0x400);
											driver.FindElementByXPath(security.Transmit[^7]).SendKeys(security.Transmit[^8]);
											await Task.Delay(0x400);

											foreach (var fieldset in driver.FindElementsByXPath(security.Transmit[^9]))
												foreach (var wrap_btn in fieldset.FindElements(By.TagName("div")))
												{
													var wrap = wrap_btn.GetAttribute("class");

													if (string.IsNullOrEmpty(wrap) is false && nameof(wrap_btn).Equals(wrap))
														foreach (var button in wrap_btn.FindElements(By.TagName("button")))
														{
															var submit = button.GetAttribute("tabindex");

															if (string.IsNullOrEmpty(submit) is false && submit[0] is '3')
															{
																button.Click();
																await Task.Delay(0x400);
																driver.Navigate().GoToUrl(System.IO.Path.Combine(story, security.Transmit[^0xA]));
																timeout.ImplicitWait = TimeSpan.FromSeconds(0xC);
																timeout.PageLoad = TimeSpan.FromSeconds(0xC);
																driver.Manage().Window.FullScreen();
																await Task.Delay(0x200);
																((IJavaScriptExecutor)driver).ExecuteScript("window.open();");

																foreach (var confirm in driver.FindElementsByXPath(security.Transmit[^0xB]))
																	foreach (var span in confirm.FindElements(By.TagName("button")))
																	{
																		var click = span.GetAttribute("class");

																		if (string.IsNullOrEmpty(click) is false && nameof(confirm).Equals(click))
																		{
																			span.Click();
																			await Task.Delay(0x400);
																			Url = driver.Url.Split('?')[^1].Split('&')[0].Trim();
																			driver.SwitchTo().Window(driver.WindowHandles[^1]).Manage().Window.FullScreen();

																			break;
																		}
																	}
																break;
															}
														}
												}
											break;
										}
									}
								break;
							}
						}
				var client = new RestClient(story)
				{
					Timeout = -1
				};
				var source = new CancellationTokenSource();
				var response = await client.ExecuteAsync(new RestRequest(security.GetToken(Url), Method.GET), source.Token);

				if (HttpStatusCode.OK.Equals(response.StatusCode))
				{
					(string, string) token = (null, null);

					foreach (var kv in JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content))
						if (string.IsNullOrEmpty(kv.Value) is false)
						{
							Url = string.Concat(kv.Key, '=', kv.Value);
							token = (kv.Key, kv.Value);
						}
					response = await client.ExecuteAsync(new RestRequest(security.GetInfomation(Url), Method.GET), source.Token);

					if (HttpStatusCode.OK.Equals(response.StatusCode))
					{
						response = await client.ExecuteAsync(new RestRequest("apis/post/attach", Method.POST).AddParameter(token.Item1, token.Item2).AddParameter("blogName", "sharecompany").AddFileBytes("uploadedfile", Properties.Resources.coreapi, "main.png"), source.Token);

						if (HttpStatusCode.OK.Equals(response.StatusCode))
						{
							StringBuilder tag = new(), content = new(security.Cloud[0]);
							var now = DateTime.Now;
							var index = 0;
							now = now.Hour < 9 ? now.AddDays(-1) : now;
							var image = string.Empty;
							var xml = new System.Xml.XmlDocument();
							xml.LoadXml(response.Content);

							foreach (var replace in xml.GetElementsByTagName("replacer"))
								if (replace is System.Xml.XmlNode node && string.IsNullOrEmpty(node.InnerText) is false)
									image = node.InnerText;

							foreach (var word in cloud.OrderByDescending(o => o.Frequency))
							{
								if (index++ < 0xA)
									tag.Append(word.Text).Append(',');

								content.Append(security.Cloud[1]).Append($"'{word.Anchor}'").Append(security.Cloud[2]).Append($"'{word.Transform}'").Append(security.Cloud[3]).Append($"'{word.Style}'").Append(security.Cloud[^3]).Append(word.Text).Append(security.Cloud[^2]);
							}
							content.Append(security.Cloud[^1]).Append(@"<br />").Append(image).Append(@"<div>").Append("<figure contenteditable='false' data-ke-type='opengraph' data-og-type='website' data-og-source-url='https://coreapi.shareinvest.net' data-og-url='https://coreapi.shareinvest.net'><a href='https://coreapi.shareinvest.net' target='_blank' rel='noopener' data-source-url='https://coreapi.shareinvest.net'><p style='text-align: center'><b><span style='font-family: Noto Serif KR'>Algorithmic Trading</span></b></p></a></figure>");

							foreach (var ns in news)
								content.Append($"<figure contenteditable='false' data-ke-type='opengraph' data-og-type='website' data-og-source-url='{ns.Key}' data-og-url='{ns.Key}'>").Append($"<a href='{ns.Key}' target='_blank' rel='noopener' data-source-url='{ns.Key}'>").Append($"<p style='text-align: center'><b><span style='font-family: Noto Serif KR'>{ns.Value}</span></b></p>").Append(@"</a>").Append(@"</figure>");

							response = await client.ExecuteAsync(new RestRequest("apis/post/write", Method.POST).AddJsonBody(JsonConvert.SerializeObject(new Catalog.Models.Tistory
							{
								Token = token.Item2,
								Type = Security.json.Split('/')[^1],
								Name = "sharecompany",
								Title = string.Concat(cn.Name, " WordCloud Tag.", cn.Code, now.ToString(Base.DateFormat)),
								Visibility = "3",
								Category = "710553",
								Publish = string.Empty,
								Slogan = string.Concat(cn.Code, cn.Name, now.ToString(Base.DateFormat)),
								Tag = tag.Remove(tag.Length - 1, 1).ToString(),
								Comment = "1",
								Password = string.Empty,
								Content = content.Append(@"</div>").ToString()

							}), Security.content_type), source.Token);
						}
					}
				}
				driver.Navigate().GoToUrl(System.IO.Path.Combine(tistory, 0x32.ToString()));
				Url = JObject.Parse(response.Content)[nameof(tistory)].ToString();
				source.Dispose();
				client.ClearHandlers();

				foreach (var header in driver.FindElementsByXPath(security.Transmit[^1]))
					foreach (var button in header.FindElements(By.TagName("button")))
					{
						var title = button.GetAttribute("title");

						if (string.IsNullOrEmpty(title) is false && menu.Equals(title))
						{
							button.Click();
							await Task.Delay(0x400);
						}
					}
				foreach (var aside in driver.FindElementsByXPath(security.Transmit[^2]))
					foreach (var div in aside.FindElements(By.TagName(nameof(aside))))
						foreach (var btn in div.FindElements(By.TagName(nameof(div))))
						{
							var attribute = btn.GetAttribute("class");

							if (string.IsNullOrEmpty(attribute) is false && attribute.StartsWith("btn-for-") && attribute.EndsWith("user"))
							{
								foreach (var a in btn.FindElements(By.TagName("a")))
								{
									var href = a.GetAttribute("href");

									if (string.IsNullOrEmpty(href) is false && driver.Url.Equals(href.Replace("#", string.Empty)))
									{
										a.Click();
										await Task.Delay(0x400);

										foreach (var button in driver.FindElementsByXPath(security.Transmit[^0xC]))
											foreach (var log in button.FindElements(By.TagName(nameof(button))))
											{
												var value = log.GetAttribute("value");

												if (string.IsNullOrEmpty(value) is false && true.ToString().ToLower().Equals(value))
												{
													log.Click();
													await Task.Delay(0x400);

													break;
												}
											}
										break;
									}
								}
								break;
							}
						}
				switch (response.StatusCode)
				{
					case HttpStatusCode.OK:
						return JsonConvert.DeserializeObject<Catalog.Models.Response>(Url);

					case HttpStatusCode.NotAcceptable:
						return (int)HttpStatusCode.NotAcceptable;
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
			}
			return null;
		}
		public Advertise(dynamic key)
		{
			security = new Security(key);
			service = ChromeDriverService.CreateDefaultService(security.Path[0]);
			service.HideCommandPromptWindow = true;
			var options = new ChromeOptions();
			options.AddArgument("--window-size=818,673");
			options.AddArgument(string.Concat("user-agent=", security.Path[^1]));
			driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(0x21));
		}
		string Url
		{
			get; set;
		}
		readonly Security security;
		readonly ChromeDriver driver;
		readonly ChromeDriverService service;
		const string menu = "메뉴";
		const string story = @"https://www.tistory.com";
		const string tistory = @"https://sharecompany.tistory.com/";
	}
}