using System;
using System.Collections.Generic;
using System.Globalization;
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
		public async Task<object> TransmitCollectedInformation(List<Catalog.KRX.Cloud> cloud, Dictionary<string, string> news)
		{
			try
			{
				StringBuilder tag = new(), contents = new(security.Cloud[0]);
				var now = DateTime.Now;
				var index = 0;
				now = now.Hour < 9 ? now.AddDays(-1) : now;

				foreach (var word in cloud.OrderByDescending(o => o.Frequency))
				{
					if (index++ < 0xA)
						tag.Append(word.Text).Append(',');

					contents.Append(security.Cloud[1]).Append($"'{word.Anchor}'").Append(security.Cloud[2]).Append($"'{word.Transform}'").Append(security.Cloud[3]).Append($"'{word.Style}'").Append(security.Cloud[^3]).Append(word.Text).Append(security.Cloud[^2]);
				}
				contents.Append(security.Cloud[^1]).Append(@"<br />").Append(@"<div>").Append("<figure contenteditable='false' data-ke-type='opengraph' data-og-type='website' data-og-source-url='https://coreapi.shareinvest.net' data-og-url='https://coreapi.shareinvest.net'><a href='https://coreapi.shareinvest.net' target='_blank' rel='noopener' data-source-url='https://coreapi.shareinvest.net'><p style='text-align: center'><b><span style='font-family: Noto Serif KR'>Algorithmic Trading</span></b></p></a></figure>");

				foreach (var ns in news)
					contents.Append($"<figure contenteditable='false' data-ke-type='opengraph' data-og-type='website' data-og-source-url='{ns.Key}' data-og-url='{ns.Key}'>").Append($"<a href='{ns.Key}' target='_blank' rel='noopener' data-source-url='{ns.Key}'>").Append($"<p style='text-align: center'><b><span style='font-family: Noto Serif KR'>{ns.Value}</span></b></p>").Append(@"</a>").Append(@"</figure>");

				switch ((await PostTiStory(contents.Append(@"</div>"), tag, "replacer", string.Concat(cn.Code, cn.Name, now.ToString(Base.DateFormat)), string.Concat(cn.Name, " WordCloud Tag.", cn.Code, now.ToString(Base.DateFormat)))).StatusCode)
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
		public async Task<object> WriteStatistics(Catalog.OpenAPI.Condition[] param, List<Catalog.Models.Theme> list)
		{
			try
			{
				var stack = new Stack<Catalog.Strategics.Condition>();
				var storage = new Stack<Catalog.Strategics.TiStory>();
				StringBuilder context = new(), tag = new();
				int length = int.MinValue, index = 1;

				foreach (var con in param)
				{
					int purchase = int.MaxValue, high = int.MinValue, low = int.MaxValue, close = int.MinValue;
					index = 0;
					length = con.Date.Length;

					foreach (var date in con.Date.OrderBy(o => o))
					{
						if (index > 0)
						{
							var find = Array.FindIndex(con.Date, o => o.Equals(date));

							if (high < con.High[find])
								high = con.High[find];

							if (low > con.Low[find])
								low = con.Low[find];
						}
						if (index++ == 1)
							purchase = con.High[Array.FindIndex(con.Date, o => o.Equals(date))];

						if (index == con.Date.Length)
							close = con.Low[Array.FindIndex(con.Date, o => o.Equals(date))];
					}
					if (length == 0xA)
						storage.Push(new Catalog.Strategics.TiStory
						{
							Code = con.Code,
							Name = con.Name,
							Theme = string.IsNullOrEmpty(con.Theme) ? string.Empty : list.Find(o => o.Index.Equals(con.Theme)).Name,
							Title = con.Title.Replace(". ", ".\n&nbsp;"),
							Purchase = purchase,
							High = high,
							HighRate = high / (double)purchase - 1,
							Low = low,
							LowRate = low / (double)purchase - 1,
							Close = close,
							RemainRate = close / (double)purchase - 1
						});
					else
						stack.Push(new Catalog.Strategics.Condition
						{
							Code = con.Code,
							Name = con.Name,
							Theme = string.IsNullOrEmpty(con.Theme) ? string.Empty : list.Find(o => o.Index.Equals(con.Theme)).Name,
							Title = con.Title.Replace(". ", ".\n&nbsp;"),
							Purchase = purchase,
							High = high,
							HighRate = high / (double)purchase - 1,
							Low = low,
							LowRate = low / (double)purchase - 1
						});
					index = 1;
				}
				if (length == 0xA)
					foreach (var story in storage.OrderByDescending(o => o.HighRate))
					{
						context.Append($"<p data-ke-size='size16'>{index}.<b>{story.Name}</b><span style='font-size:small;'>({story.Code})</span></p><p data-ke-size='size14'>&nbsp;Purchase_{story.Purchase:C0}</p><p data-ke-size='size14'>&nbsp;High_{story.High:C0}</p><p data-ke-size='size14'>&nbsp;Maximum_Return_<span style='color:{(story.HighRate > 0 ? "#FF0000" : "#0000FF")};'>{Math.Abs(story.HighRate):P2}</span></p><p data-ke-size='size14'>&nbsp;Low_{story.Low:C0}</p><p data-ke-size='size14'>&nbsp;Maximum_Loss_Rate_<span style='color:{(story.LowRate > 0 ? "#FF0000" : "#0000FF")};'>{Math.Abs(story.LowRate):P2}</span></p><p data-ke-size='size14'>&nbsp;Remain_{story.Close:C0}</p><p data-ke-size='size14'>&nbsp;Liquidation_Return_Rate_<span style='color:{(story.RemainRate > 0 ? "#FF0000" : "#0000FF")};'>{Math.Abs(story.RemainRate):P2}</span></p>");

						if (string.IsNullOrEmpty(story.Theme) is false)
							context.Append($"<p data-ke-size='size14'>&nbsp;<strong>{story.Theme}</strong></p>").Append($"<p data-ke-size='size14' style='white-space:pre-line;'>&nbsp;{story.Title}</p>");

						if (index < 0xB)
							tag.Append(story.Name).Append(',');

						if (index++ < storage.Count)
							context.Append("<hr contenteditable='false' data-ke-type='horizontalRule' data-ke-style='style6'/>");
					}
				else
					foreach (var story in stack.OrderByDescending(o => o.HighRate))
					{
						context.Append($"<p data-ke-size='size16'>{index}.<b>{story.Name}</b><span style='font-size:small;'>({story.Code})</span></p>");

						if (string.IsNullOrEmpty(story.Theme) is false)
							context.Append($"<p data-ke-size='size14'>&nbsp;<strong>{story.Theme}</strong></p>");

						if (length > 1)
							context.Append($"<p data-ke-size='size14'>&nbsp;Purchase_{story.Purchase:C0}</p><p data-ke-size='size14'>&nbsp;High_{story.High:C0}</p><p data-ke-size='size14'>&nbsp;Maximum_Return_<span style='color:{(story.HighRate > 0 ? "#FF0000" : "#0000FF")};'>{Math.Abs(story.HighRate):P2}</span></p><p data-ke-size='size14'>&nbsp;Low_{story.Low:C0}</p><p data-ke-size='size14'>&nbsp;Maximum_Loss_Rate_<span style='color:{(story.LowRate > 0 ? "#FF0000" : "#0000FF")};'>{Math.Abs(story.LowRate):P2}</span></p>");

						if (index < 0xB)
							tag.Append(story.Name).Append(',');

						if (string.IsNullOrEmpty(story.Title) is false)
							context.Append($"<p data-ke-size='size14' style='white-space:pre-line;'>&nbsp;{story.Title}</p>");

						if (index++ < stack.Count)
							context.Append("<hr contenteditable='false' data-ke-type='horizontalRule' data-ke-style='style6'/>");
					}
				context.Append("<hr contenteditable='false' data-ke-type='horizontalRule' data-ke-style='style1'/>").Append($"<p data-ke-size='size18'><b>☞{date}</b></p>").Append($"<figure id='og_1618408583729' contenteditable='false' data-ke-type='opengraph' data-og-type='website' data-og-title='Algorithmic Trading' data-og-description='the profit-generating model using Algorithms.' data-og-host='coreapi.shareinvest.net' data-og-source-url='https://coreapi.shareinvest.net' data-og-url='https://coreapi.shareinvest.net' data-og-image='https://scrap.kakaocdn.net/dn/cGYG7I/hyJSmdXhpa/r7rd0tlAsaOJiKWEvo49q0/img.jpg?width=320&amp;height=290&amp;face=61_24_196_171'><a href='https://coreapi.shareinvest.net' target='_blank' rel='noopener' data-source-url='https://coreapi.shareinvest.net'><div class='og-image' style='background-image:url({net});'>&nbsp;</div><div class='og-text'><p class='og-title'>Algorithmic Trading</p><p class='og-desc'>the profit-generating model using Algorithms.</p><p class='og-host'>coreapi.shareinvest.net</p></div></a></figure><blockquote data-ke-size='size14' data-ke-style='style2'>매수 가격은 추천 익일 고가로 산정하며 최대 보유기간은 매수일로부터 9일로 한다.</blockquote>");

				switch ((await PostTiStory(context, tag, string.Empty, "Algorithmic Trading", string.Concat("알고리즘 트레이딩 수익률 Tag.", date))).StatusCode)
				{
					case HttpStatusCode.OK:
						return length == 0xA ? storage : stack;

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
		public async Task WriteStatistics(IEnumerable<Catalog.Models.Rotation> param, Dictionary<string, Tuple<string, Catalog.Dart.TiStory>> theme)
		{
			try
			{
				double max = param.Max(o => o.MaxReturn), min = param.Min(o => o.MaxLoss);
				int index = 0, mark = (int)(0x64 * (max > Math.Abs(min) ? max : Math.Abs(min)));
				var nor = new SecondaryIndicators.Normalization(max, min);
				StringBuilder contents = new("<table style='border-collapse: collapse; width: 100%; border: 1px solid #dcdcdc'><thead>"), tags = new();
				contents.Append("<tr>").Append("<th style='color: gainsboro; font-size: small; font-style: italic; width: 15%'>Distribution</th>").Append("<th style='background: linear-gradient(to right, blue 0%, black 50%, red 100%) border-box' colspan='10'>").Append($"<span style='color: #ffffff; display: inline-block; text-align: left; width:33%'>{mark}％←</span>").Append("<span style='color: #ffffff; display: inline-block; text-align: center; width:30%'>0％</span>").Append($"<span style='color: #ffffff; display: inline-block; text-align: right; width:33%'>→{mark}％</span>").Append("</th>").Append("</tr>").Append("</thead>").Append("<tbody>");

				foreach (var ro in param)
				{
					double loss = nor.Normalize(ro.MaxLoss), liquidation = nor.Normalize(ro.Liquidation), revenue = nor.Normalize(ro.MaxReturn);
					contents.Append("<tr>").Append($"<th title='{theme[ro.Code].Item1}' style='width: 15%'>{ro.Code}</th>").Append("<td colspan='10' style='border-top: 1px solid #ffffff'>").Append($"<span style='color: rgb(0, 0, 255); display: inline-block; text-align: right; width: {loss:P9}' title='{Math.Abs(ro.MaxLoss):P2}'>↑</span>").Append($"<span style='color: rgb(0, 0, 0); display: inline-block; text-align: right; width: {liquidation - loss:P9}' title='{Math.Abs(ro.Liquidation):P2}'>↑</span>").Append($"<span style='color: rgb(255, 0, 0); display: inline-block; text-align: right; width: {revenue - liquidation:P9}' title='{ro.MaxReturn:P2}'>↑</span>");

					if (index++ > 0)
						contents.Append($"<span style='cursor: pointer; color: rgb(255, 255, 255); display: inline-block; text-align: left' title='{(theme[ro.Code].Item2 is Catalog.Dart.TiStory ti ? ti.Index : string.Empty)}'>&nbsp;</span>");

					contents.Append("</td>").Append("</tr>");
				}
				contents.Append("</tbody>").Append("</table>").Append("<hr contenteditable='false' data-ke-type='horizontalRule' data-ke-style='style1'/>").Append($"<figure id='og_1618408583729' contenteditable='false' data-ke-type='opengraph' data-og-type='website' data-og-title='Algorithmic Trading' data-og-description='the profit-generating model using Algorithms.' data-og-host='coreapi.shareinvest.net' data-og-source-url='https://coreapi.shareinvest.net' data-og-url='https://coreapi.shareinvest.net' data-og-image='https://scrap.kakaocdn.net/dn/cGYG7I/hyJSmdXhpa/r7rd0tlAsaOJiKWEvo49q0/img.jpg?width=320&amp;height=290&amp;face=61_24_196_171'><a href='https://coreapi.shareinvest.net' target='_blank' rel='noopener' data-source-url='https://coreapi.shareinvest.net'><div class='og-image' style='background-image:url({net});'>&nbsp;</div><div class='og-text'><p class='og-title'>Algorithmic Trading</p><p class='og-desc'>the profit-generating model using Algorithms.</p><p class='og-host'>coreapi.shareinvest.net</p></div></a></figure>");
				index = 1;

				foreach (var story in param)
				{
					contents.Append($"<p data-ke-size='size16'>{index}.<b>{theme[story.Code].Item1}</b><span style='font-size:small;'>({story.Code})</span></p><p data-ke-size='size14'>&nbsp;Purchase_{story.Purchase:C0}</p><p data-ke-size='size14'>&nbsp;High_{story.High:C0}</p><p data-ke-size='size14'>&nbsp;Maximum_Return_<span style='color:{(story.MaxReturn > 0 ? "#FF0000" : "#0000FF")};'>{Math.Abs(story.MaxReturn):P2}</span></p><p data-ke-size='size14'>&nbsp;Low_{story.Low:C0}</p><p data-ke-size='size14'>&nbsp;Maximum_Loss_Rate_<span style='color:{(story.MaxLoss > 0 ? "#FF0000" : "#0000FF")};'>{Math.Abs(story.MaxLoss):P2}</span></p><p data-ke-size='size14'>&nbsp;Remain_{story.Close:C0}</p><p data-ke-size='size14'>&nbsp;Liquidation_Return_Rate_<span style='color:{(story.Liquidation > 0 ? "#FF0000" : "#0000FF")};'>{Math.Abs(story.Liquidation):P2}</span></p>");

					if (theme[story.Code].Item2 is Catalog.Dart.TiStory ts)
						contents.Append($"<p data-ke-size='size14'>&nbsp;<strong>{ts.Index}</strong></p>").Append($"<p data-ke-size='size14' style='white-space:pre-line;'>&nbsp;{ts.Title.Replace(". ", ".\n&nbsp;")}</p>");

					if (index < 0xB)
						tags.Append(theme[story.Code].Item1).Append(',');

					var sb = new StringBuilder();

					foreach (var date in story.Date.Split(';'))
						if (DateTime.TryParseExact(date, Base.DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime time))
							sb.Append(time.ToLongDateString()).Append('，').Append("&nbsp;");

					contents.Append($"<p data-ke-size='size18'><b>☞{sb.Remove(sb.Length - 7, 7)}</b></p>");

					if (index > 0x31)
						break;

					if (index++ < theme.Count)
						contents.Append("<hr contenteditable='false' data-ke-type='horizontalRule' data-ke-style='style6'/>");
				}
				contents.Append("<hr contenteditable='false' data-ke-type='horizontalRule' data-ke-style='style1'/>").Append("<blockquote data-ke-size='size14' data-ke-style='style2'>매수 가격은 추천 익일 고가로 산정하며 최대 보유기간은 매수일로부터 9일로 한다.</blockquote>");
				await PostTiStory(contents, tags, string.Empty, "Data Analytics", $"알고리즘 시스템 트레이딩 수익률 통계 Tag.{DateTime.Now.ToLongDateString()}");
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
		public Advertise(string date, dynamic key)
		{
			security = new Security(key);
			service = ChromeDriverService.CreateDefaultService(security.Path[0]);
			service.HideCommandPromptWindow = true;
			var options = new ChromeOptions();
			options.AddArgument("--window-size=800,450");
			options.AddArgument(string.Concat("user-agent=", security.Path[^1]));
			driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(0x21));

			if (DateTime.TryParseExact(date, Base.DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime time))
				this.date = time.ToLongDateString();
		}
		public Advertise(Catalog.Models.Codes cn, dynamic key)
		{
			security = new Security(key);
			service = ChromeDriverService.CreateDefaultService(security.Path[0]);
			service.HideCommandPromptWindow = true;
			var options = new ChromeOptions();
			options.AddArgument("--window-size=800,450");
			options.AddArgument(string.Concat("user-agent=", security.Path[^1]));

			if ((security.IsInsiders || security.IsHome) is false)
				options.AddArguments("headless");

			this.cn = cn;
			driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(0x21));
		}
		async Task<IRestResponse> PostTiStory(StringBuilder contents, StringBuilder tag, string image, string slogan, string title)
		{
			await LogIn();
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
					response = await client.ExecuteAsync(new RestRequest("apis/post/attach", Method.POST).AddParameter(token.Item1, token.Item2).AddParameter("blogName", "sharecompany").AddFileBytes("uploadedfile", resources[random.Next(0, resources.Count)], "main.png"), source.Token);

					if (string.IsNullOrEmpty(image) is false)
					{
						var xml = new System.Xml.XmlDocument();
						xml.LoadXml(response.Content);

						foreach (var replace in xml.GetElementsByTagName(image))
							if (replace is System.Xml.XmlNode node && string.IsNullOrEmpty(node.InnerText) is false)
								image = node.InnerText;
					}
					if (HttpStatusCode.OK.Equals(response.StatusCode))
						response = await client.ExecuteAsync(new RestRequest("apis/post/write", Method.POST).AddJsonBody(JsonConvert.SerializeObject(new Catalog.Models.Tistory
						{
							Token = token.Item2,
							Type = Security.json.Split('/')[^1],
							Name = "sharecompany",
							Title = title,
							Visibility = "3",
							Category = "710553",
							Publish = string.Empty,
							Slogan = slogan,
							Tag = tag.Remove(tag.Length - 1, 1).ToString(),
							Comment = "1",
							Password = string.Empty,
							Content = (string.IsNullOrEmpty(image) ? contents : contents.Append(image)).ToString()

						}), Security.content_type), source.Token);
				}
			}
			Url = JObject.Parse(response.Content)[nameof(tistory)].ToString();
			source.Dispose();
			client.ClearHandlers();
			await LogOut();

			return response;
		}
		async Task LogOut()
		{
			driver.Navigate().GoToUrl(System.IO.Path.Combine(tistory, 0x32.ToString()));

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
												try
												{
													log.Click();
												}
												catch (Exception ex)
												{
													Base.SendMessage(GetType(), nameof(this.LogOut), ex.StackTrace);
												}
												finally
												{
													await Task.Delay(0x400);
												}
												break;
											}
										}
									break;
								}
							}
							break;
						}
					}
		}
		async Task LogIn()
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
		}
		string Url
		{
			get; set;
		}
		readonly string date;
		readonly Random random = new(Guid.NewGuid().GetHashCode());
		readonly Security security;
		readonly ChromeDriver driver;
		readonly ChromeDriverService service;
		readonly List<byte[]> resources = new() { Properties.Resources.SE_44c2591d_835c_45d6_a9cd_985d6e3aaa13, Properties.Resources.SE_a3017409_6f36_4eca_ace3_f363af2a686c, Properties.Resources.SE_a8f95f48_320a_4e70_98d6_f4740b323222, Properties.Resources.SE_f927a675_1a0d_4f30_b62e_92efb8c15ad5, Properties.Resources.SE_8e9889e5_413d_4aee_978d_6247142755d3, Properties.Resources.SE_7d12c943_5220_47b3_b6d3_712164afb03f, Properties.Resources.SE_63a3542b_8637_4fed_ba9c_2bf970a6321d, Properties.Resources.SE_52af2672_5b2c_44ee_9d74_892136bde030, Properties.Resources.SE_46c4cbab_4d1a_4e17_a42d_31365b80f22f, Properties.Resources.SE_14a7635c_0d85_40e9_83a9_cbc7d85d36a7, Properties.Resources._20210311_Stocks, Properties.Resources._20210311_TagCloud, Properties.Resources._20210311_Portfolio, Properties.Resources._20210311_Theme };
		readonly Catalog.Models.Codes cn;
		const string menu = "메뉴";
		const string story = @"https://www.tistory.com";
		const string tistory = @"https://sharecompany.tistory.com/";
		const string net = @"https://scrap.kakaocdn.net/dn/cGYG7I/hyJSmdXhpa/r7rd0tlAsaOJiKWEvo49q0/img.jpg?width=320&amp;height=290&amp;face=61_24_196_171";
	}
}