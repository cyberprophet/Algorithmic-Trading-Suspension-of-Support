using System;
using System.Collections.Generic;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ShareInvest.Client
{
    public class IncorporatedStocks
    {
        public Stack<Catalog.Request.IncorporatedStocks> OnReceiveSequentially(int page)
        {
            try
            {
                driver.Navigate().GoToUrl(security.RequestParameter(page));
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(7);
                var stack = new Stack<Catalog.Request.IncorporatedStocks>();
                var date = DateTime.Now.ToString("yyMMdd");

                foreach (var tr in driver.FindElementByXPath("/html/body/div/table[1]").FindElement(By.TagName("tbody")).FindElements(By.TagName("tr")))
                {
                    var index = 0;
                    var stock = new Catalog.Request.IncorporatedStocks
                    {
                        Date = date,
                        Market = 'P'
                    };
                    foreach (var a in tr.FindElements(By.TagName("a")))
                    {
                        var code = a.GetAttribute("href").Split('=')[1];

                        if (code.Length == 6)
                        {
                            stock.Code = code.Trim();
                            stock.Name = a.Text.Trim();
                        }
                    }
                    foreach (var td in tr.FindElements(By.TagName("td")))
                        if (index++ == 6 && int.TryParse(td.Text.Replace(",", string.Empty), out int total))
                        {
                            stock.Capitalization = total;
                            stack.Push(stock);
                        }
                }
                return stack;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
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
        public IncorporatedStocks(dynamic key)
        {
            security = new Security(key);

            if (security.GrantAccess)
            {
                service = ChromeDriverService.CreateDefaultService(security.Path);
                service.HideCommandPromptWindow = true;
                options = new ChromeOptions();
                options.AddArgument("--window-size=631,493");
                options.AddArgument(string.Concat("user-agent=", security.Agent));
                driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(0xA));
            }
        }
        readonly Security security;
        readonly ChromeDriver driver;
        readonly ChromeOptions options;
        readonly ChromeDriverService service;
    }
}