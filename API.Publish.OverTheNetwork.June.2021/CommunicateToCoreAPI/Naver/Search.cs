using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

using ShareInvest.Client;

namespace ShareInvest.Naver
{
	public class Search
	{
		public async Task<(string, Stack<string>)> BrowseTheSite(string url)
		{
			try
			{
				driver.Navigate().GoToUrl(url);
				var timeout = driver.Manage().Timeouts();
				timeout.ImplicitWait = TimeSpan.FromSeconds(0xC);
				timeout.PageLoad = TimeSpan.FromSeconds(0xC);
				var title = string.Empty;
				var contents = new Stack<string>();
				await Task.Delay(0x200);
				driver.Manage().Window.FullScreen();

				switch (url.Split('.')[0][^4..])
				{
					case blog when driver.FindElement(By.TagName(frame)) is IWebElement web:
						var ds = driver.SwitchTo().Frame(web);

						foreach (var ct in ds.FindElements(By.Id($"post-view{url.Split('/')[^1]}")))
						{
							if (string.IsNullOrEmpty(title))
								title = ds.Title.Split(':')[0].Replace("..", string.Empty).Trim();

							contents.Push(ct.Text);
						}
						break;

					case cafe when driver.WindowHandles.Count == 1:

						break;

					case post:

						break;
				}
				new Actions(driver).SendKeys(Keys.Escape).Perform();

				if (contents.Count > 0 && string.IsNullOrEmpty(title) is false)
					return (title, contents);
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
			return (null, null);
		}
		public async Task<(List<Catalog.KRX.Cloud>, Dictionary<string, string>)> VisualizeTheResultsOfAnAnalysis(string keyword)
		{
			try
			{
				driver.Navigate().GoToUrl(Security.Kinds);
				var timeout = driver.Manage().Timeouts();
				timeout.ImplicitWait = TimeSpan.FromSeconds(0xC);
				timeout.PageLoad = TimeSpan.FromSeconds(0xC);
				await Task.Delay(0x200);
				IAction action = new Actions(driver).SendKeys(Keys.ArrowDown).Build(), enter = new Actions(driver).SendKeys(Keys.Enter).Build(), up = new Actions(driver).SendKeys(Keys.ArrowUp).Build();
				var list = new List<Catalog.KRX.Cloud>();
				var dictionary = new Dictionary<string, string>();
				driver.Manage().Window.FullScreen();

				foreach (var key in driver.FindElementsByXPath(Security.Click[^1]))
				{
					key.SendKeys(keyword);
					enter.Perform();
					await Task.Delay(0x200);
				}
				foreach (var select in driver.FindElementsByXPath(Security.Click[^2]))
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
				for (int i = Security.Click.Length - 2; i > 3; i--)
				{
					await Task.Delay(0xC00);
					Web = driver.FindElementByXPath(Security.Click[i - 1]);
					Web.Click();
					up.Perform();
				}
				foreach (var input in Web.FindElements(By.TagName("input")))
				{
					await Task.Delay(0x200);
					var id = input.GetAttribute("id");

					if (Array.Exists(Security.Click, o => o.Substring(9, 5).Equals(id)))
					{
						foreach (var label in Web.FindElements(By.TagName("label")))
							if (id.Equals(label.GetAttribute("for")))
							{
								label.Click();
								await Task.Delay(0x200);
							}
						if (Security.Click[0].Substring(9, 5).Equals(id))
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

					if (href.StartsWith(Security.News) is false && dictionary.TryGetValue(news, out string detail) && dictionary.Remove(news))
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
		public async Task<Queue<string>> SearchForKeyword(string param, int length)
		{
			try
			{
				driver.Navigate().GoToUrl(Security.GoToKeyword(param));
				driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0xC);
				driver.Manage().Window.FullScreen();
				var action = new Actions(driver).SendKeys(Keys.ArrowDown).Build();
				var stack = new Stack<IWebElement>();
				var queue = new Queue<string>();

				while (length-- > 0)
				{
					await Task.Delay(0x100);
					action.Perform();

					if ((driver as IJavaScriptExecutor).ExecuteScript(Base.Command) is 0x64L or > 1e+2)
						break;
				}
				foreach (var x in driver.FindElementsByXPath("//*[@id='main_pack']/section/div/div[2]/panel-list/div/more-contents/div/ul"))
					foreach (var ul in x.FindElements(By.TagName("li")))
						foreach (var div in ul.FindElements(By.TagName("div")))
						{
							var attribute = div.GetAttribute("data-cr-rank");

							if (string.IsNullOrEmpty(attribute) is false)
								stack.Push(div);
						}
				while (stack.TryPop(out IWebElement web))
					foreach (var a in web.FindElements(By.TagName("a")))
					{
						var confirm = a.GetAttribute("class");

						if (dsc.Equals(confirm))
							queue.Enqueue(a.GetAttribute("href"));
					}
				new Actions(driver).SendKeys(Keys.Escape).Perform();

				if (queue.Count > 0)
					return queue;
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
		readonly ChromeDriver driver;
		readonly ChromeDriverService service;
	}
}