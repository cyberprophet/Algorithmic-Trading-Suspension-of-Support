using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.GoblinBatContext;

namespace ShareInvest.Strategy
{
    public partial class Retrieve : CallUpStatisticalAnalysis
    {
        public long OnReceiveRepositoryID(Catalog.XingAPI.Specify[] specifies) => GetRepositoryID(specifies);
        public Dictionary<DateTime, string> OnReceiveInformation(long number) => GetInformation(number);
        public Catalog.XingAPI.Specify[] OnReceiveStrategy(long index) => GetStrategy(index);
        public Retrieve(string key) : base(key) => secret = new Secrets(new Queue<Models.Memorize>(1024));
        public Catalog.XingAPI.Specify[] GetUserStrategy()
        {
            var identity = GetUserIdentify();
            var indexes = GetBestStrategyRecommend();

            if (identity != null)
                foreach (var kv in indexes)
                    if (ulong.TryParse(identity[0], out ulong assets) && int.TryParse(identity[2], out int commission) && double.TryParse(identity[3], out double rate))
                    {
                        var strategy = GetStrategy(kv.Key);
                        var check = strategy[0];

                        if (check.Assets == assets * ar && check.Code.Equals(identity[1]) && check.Commission == commission / cr && check.MarginRate == rate / mr && check.Strategy.Equals(identity[4]))
                        {
                            if (identity[5].Equals("A") == false)
                                switch (identity[5])
                                {
                                    case "T":
                                        if (check.RollOver == false)
                                            continue;

                                        break;

                                    case "F":
                                        if (check.RollOver == true)
                                            continue;

                                        break;
                                }
                            return strategy;
                        }
                    }
            return GetStrategy(indexes.OrderByDescending(o => o.Value).First().Key);
        }
        public void SetInitialzeTheCode(string code)
        {
            if (Chart == null && Quotes == null)
            {
                Chart = GetChart(code);
                Quotes = GetQuotes(code);
            }
        }
        public List<long> SetInitialzeTheCode()
        {
            Code = GetStrategy();
            SetInitialzeTheCode(Code);

            return GetStrategy("16.2");
        }
        public void SetInitializeTheChart()
        {
            if (Chart != null)
            {
                Chart.Clear();
                Chart = null;
            }
            if (Quotes != null)
            {
                Quotes.Clear();
                Quotes = null;
            }
        }
        public bool GetDuplicateResults(long index)
        {
            var now = DateTime.Now;

            switch (now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    if (now.Hour < 16)
                        now = now.AddDays(-3);

                    break;

                case DayOfWeek.Sunday:
                    now = now.AddDays(-2);
                    break;

                case DayOfWeek.Saturday:
                    now = now.AddDays(-1);
                    break;

                default:
                    if (now.Hour < 16 || secret.GetHoliday(now))
                        now = now.AddDays(-1);

                    break;
            }
            return GetDuplicateResults(index, now.ToString(date));
        }
        public string GetDate(string code)
        {
            if (DateTime.TryParseExact(SetDate(code).Substring(0, 12), format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
                return string.Concat(date.ToLongDateString(), " ", date.ToShortTimeString());

            return string.Empty;
        }
        protected override string GetConvertCode(string code)
        {
            if (Code.Substring(0, 3).Equals(code.Substring(0, 3)) && Code.Substring(5).Equals(code.Substring(3)))
                return Code;

            return code;
        }
        public async void SetIdentify(Setting setting) => await SetIndentify(setting);
        public static string Code
        {
            get; set;
        }
        public static string Date
        {
            get
            {
                if (DateTime.TryParseExact(Quotes.Last().Time.Substring(0, 12), format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
                    return string.Concat(date.ToLongDateString(), " ", date.ToShortTimeString());

                else
                    return string.Empty;
            }
        }
        protected internal static Queue<Chart> Chart
        {
            get; private set;
        }
        protected internal static Queue<Quotes> Quotes
        {
            get; private set;
        }
        readonly Secrets secret;
        const string date = "yyMMdd";
        const string format = "yyMMddHHmmss";
    }
}