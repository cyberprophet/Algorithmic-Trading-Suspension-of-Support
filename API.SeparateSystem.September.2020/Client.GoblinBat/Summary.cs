using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using OpenQA.Selenium.Chrome;

namespace ShareInvest.Client
{
    public class Summary
    {
        public Queue<Catalog.Request.FinancialStatement> GetContextAsync(string code)
        {
            try
            {
                driver.Navigate().GoToUrl(string.Concat(security.Info, '/', security.RequestParameter(code)));
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                driver.FindElementByXPath(security.Cns).Click();
                string[] quarter = new string[8], name = new string[0x21], value = new string[8];
                var list = new List<string[]>();
                var queue = new Queue<Catalog.Request.FinancialStatement>();
                var count = 0;

                foreach (var str in driver.PageSource.Split(security.Summary, StringSplitOptions.RemoveEmptyEntries)[1].Split(security.T1, StringSplitOptions.RemoveEmptyEntries))
                {
                    var param = str.Replace("\t", string.Empty).Replace("\r\n", string.Empty).Replace(security.Replace[0], string.Empty);
                    var empty = false;
                    var index = 0;

                    if (param.StartsWith(security.Replace[1]))
                        foreach (var num in param.Split(security.T2, StringSplitOptions.None))
                        {
                            var array = num.ToCharArray();

                            if (index < 0xA && (string.IsNullOrEmpty(num) || num.StartsWith("-") && char.IsDigit(array[1]) || char.IsDigit(array[0]) || char.IsLetter(array[0])) && (string.IsNullOrEmpty(num) == false || empty))
                            {
                                param = num.Replace(security.Replace[2], string.Empty);

                                if (index++ == 0)
                                {
                                    name[count] = param;

                                    if (count++ > 0)
                                        list.Add(value);

                                    value = new string[8];
                                }
                                else
                                    value[index - 2] = param;
                            }
                            empty = num.StartsWith(security.Replace[3]);
                        }
                    else if (param.StartsWith(security.Replace[4]))
                        foreach (var date in param.Split(security.T3, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var array = date.ToCharArray();

                            if (index < 8 && array.Length > 4 && char.IsDigit(array[0]) && char.IsDigit(array[1]) && char.IsDigit(array[2]) && char.IsDigit(array[3]))
                                quarter[index++] = (date.EndsWith("(E)") ? date : date.Insert(date.Length, "(A)")).Substring(2).Replace('/', '.');
                        }
                }
                list.Add(value);
                driver.Close();
                driver.Dispose();

                for (count = 0; count < quarter.Length; count++)
                {
                    var dictionary = new Dictionary<string, string>()
                    {
                        { "Code", code },
                        { "Date", quarter[count] }
                    };
                    for (int i = 0; i < name.Length; i++)
                        dictionary[name[i]] = list[i][count];

                    var dart = JsonConvert.DeserializeObject<Catalog.Dart.FinancialStatement>(JsonConvert.SerializeObject(dictionary));
                    queue.Enqueue(new Catalog.Request.FinancialStatement
                    {
                        Code = dart.Code,
                        Date = dart.Date,
                        Revenues = dart.Revenues,
                        IncomeFromOperation = dart.IncomeFromOperation,
                        IncomeFromOperations = dart.IncomeFromOperations,
                        ProfitFromContinuingOperations = dart.ProfitFromContinuingOperations,
                        NetIncome = dart.NetIncome,
                        ControllingNetIncome = dart.ControllingNetIncome,
                        NonControllingNetIncome = dart.NonControllingNetIncome,
                        TotalAssets = dart.TotalAssets,
                        TotalLiabilites = dart.TotalLiabilites,
                        TotalEquity = dart.TotalEquity,
                        ControllingEquity = dart.ControllingEquity,
                        NonControllingEquity = dart.NonControllingEquity,
                        EquityCapital = dart.EquityCapital,
                        OperatingActivities = dart.OperatingActivities,
                        InvestingActivities = dart.InvestingActivities,
                        FinancingActivities = dart.FinancingActivities,
                        CAPEX = dart.CAPEX,
                        FCF = dart.FCF,
                        InterestAccruingLiabilities = dart.InterestAccruingLiabilities,
                        OperatingMargin = dart.OperatingMargin,
                        NetMargin = dart.NetMargin,
                        ROE = dart.ROE,
                        ROA = dart.ROA,
                        DebtRatio = dart.DebtRatio,
                        RetentionRatio = dart.RetentionRatio,
                        EPS = dart.EPS,
                        PER = dart.PER,
                        BPS = dart.BPS,
                        PBR = dart.PBR,
                        DPS = dart.DPS,
                        DividendYield = dart.DividendYield,
                        PayoutRatio = dart.PayoutRatio,
                        IssuedStocks = dart.IssuedStocks
                    });
                }
                return queue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }
        public Summary(dynamic key)
        {
            security = new Security(key);

            if (security.GrantAccess)
            {
                service = ChromeDriverService.CreateDefaultService(security.Path);
                service.HideCommandPromptWindow = true;
                options = new ChromeOptions();
                options.AddArgument("headless");
                driver = new ChromeDriver(service, options);
            }
        }
        readonly Security security;
        readonly ChromeDriver driver;
        readonly ChromeOptions options;
        readonly ChromeDriverService service;
    }
}