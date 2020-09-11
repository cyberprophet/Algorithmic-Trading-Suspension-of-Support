using System;
using System.Threading.Tasks;

using OpenQA.Selenium.Chrome;

namespace ShareInvest.Client
{
    public class Advertise
    {
        public async Task StartAdvertisingInTheDataCollectionSection(DateTime now)
        {
            try
            {
                driver.Navigate().GoToUrl(security.RequestShareInvestBlog(random.Next(0, now.Day + now.Month + now.Year + DateTime.DaysInMonth(now.Year, now.Month) + now.Second - 0x7D0)));
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0xA);
                await Task.Delay(0x2715);
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
        }
        public Advertise(dynamic key)
        {
            security = new Security(key);
            service = ChromeDriverService.CreateDefaultService(security.Path);
            service.HideCommandPromptWindow = true;
            options = new ChromeOptions();
            options.AddArgument("--window-size=818,673");
            options.AddArgument(string.Concat("user-agent=", security.Agent));
            driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(0x21));
            random = new Random();
        }
        readonly Random random;
        readonly Security security;
        readonly ChromeDriver driver;
        readonly ChromeOptions options;
        readonly ChromeDriverService service;
    }
}