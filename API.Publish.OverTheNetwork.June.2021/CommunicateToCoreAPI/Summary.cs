using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using OpenQA.Selenium.Chrome;

using ShareInvest.Catalog;

namespace ShareInvest.Client
{
	public sealed class Summary
	{
		public Queue<FinancialStatement> GetContext(string code, int day)
		{
			try
			{
				driver.Navigate().GoToUrl(security.RequestTheSummmaryAddress(code));
				var timeout = driver.Manage().Timeouts();
				timeout.ImplicitWait = TimeSpan.FromSeconds(7);
				timeout.PageLoad = TimeSpan.FromSeconds(0xC);
				driver.FindElementByXPath(security.Cns(day))?.Click();
				string[] quarter = new string[8], name = new string[0x21], value = new string[8];
				var list = new List<string[]>();
				var queue = new Queue<FinancialStatement>();
				int count = 0, index;

				foreach (var str in driver.PageSource.Split(security.Summary, StringSplitOptions.RemoveEmptyEntries)[1].Split(security.T1, StringSplitOptions.RemoveEmptyEntries))
				{
					var param = str.Replace("\t", string.Empty).Replace("\r\n", string.Empty).Replace(security.Replace[0], string.Empty);
					var empty = false;
					index = 0;

					if (param.StartsWith(security.Replace[1]))
						foreach (var num in param.Split(security.T2, StringSplitOptions.None))
						{
							var array = num.ToCharArray();

							if (index < 0xA
								&& (string.IsNullOrEmpty(num) || num.StartsWith("-") && char.IsDigit(array[1]) || char.IsDigit(array[0]) || char.IsLetter(array[0]))
								&& (string.IsNullOrEmpty(num) == false || empty))
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
								quarter[index++] = (date.EndsWith("(E)") ? date : date.Insert(date.Length, "(A)"))[2..].Replace('/', '.');
						}
				}
				list.Add(value);
				index = 0;

				for (count = 0; count < quarter.Length; count++)
					if (string.IsNullOrEmpty(quarter[quarter.Length - count - 1]) == false)
					{
						var dictionary = new Dictionary<string, string>()
						{
							{ "Code", code },
							{ "Date", quarter[index++] }
						};
						for (int i = 0; i < name.Length; i++)
						{
							var temp = list[i][count];
							dictionary[name[i]] = temp.Length > 3 ? temp.Replace(",", string.Empty) : temp;
						}
						var dart = JsonConvert.DeserializeObject<Catalog.Dart.FinancialStatement>(JsonConvert.SerializeObject(dictionary));
						queue.Enqueue(new FinancialStatement
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
		public Summary(dynamic key)
		{
			security = new Security(key);

			if (security.GrantAccess)
			{
				service = ChromeDriverService.CreateDefaultService(security.Path[0]);
				service.HideCommandPromptWindow = true;
				var options = new ChromeOptions();
				options.AddArgument(string.Concat("user-agent=", security.Path[^1]));

				if (security.IsInsiders is false)
					options.AddArguments("headless");

				driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(0x40));
			}
		}
		readonly Security security;
		readonly ChromeDriver driver;
		readonly ChromeDriverService service;
	}
}