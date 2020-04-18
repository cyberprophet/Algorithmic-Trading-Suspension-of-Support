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
        public Retrieve(string key) : base(key) => secret = new Secrets(new Queue<Models.Memorize>(1024));
        public Dictionary<DateTime, string> OnReceiveInformation(long number) => GetInformation(number);
        public long OnReceiveRepositoryID(Catalog.XingAPI.Specify[] specifies) => GetRepositoryID(specifies);
        public Catalog.XingAPI.Specify[] OnReceiveStrategy(long index) => GetStrategy(index);
        public Catalog.XingAPI.Specify[] GetUserStrategy() => GetStrategy(GetBestStrategyRecommend());
        public List<long> SetInitialzeTheCode(bool identify)
        {
            Code = GetStrategy();
            SetInitialzeTheCode(Code);

            if (identify && DateTime.Now.Hour < 18 && (DateTime.Now.Hour > 15 || DateTime.Now.Hour == 15 && DateTime.Now.Minute > 45) && DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) == false && DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday) == false)
            {
                var list = GetUserIdentify(DateTime.Now.AddDays(-1).ToString(date));

                if (list != null)
                    return list;
            }
            return GetStrategy(marginRate);
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
        public bool SetStatisticalStorage()
        {
            var memo = Secrets.Memorizes;
            Secrets.Memorizes.Clear();

            if (memo.Count > 0)
                SetStatisticalStorage(memo).Wait();

            return true;
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
        void SetInitialzeTheCode(string code)
        {
            if (Chart == null && Quotes == null)
            {
                Chart = GetChart(code);
                Quotes = GetQuotes(code);
            }
        }
        readonly Secrets secret;
        const string date = "yyMMdd";
        const string format = "yyMMddHHmmss";
    }
}