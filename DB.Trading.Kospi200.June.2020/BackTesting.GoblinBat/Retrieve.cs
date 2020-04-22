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
        public Retrieve(string key) : base(key) => secret = new Secrets(new Queue<Models.ImitationGames>(1024));
        public Dictionary<DateTime, string> OnReceiveInformation(Catalog.DataBase.ImitationGame number) => GetInformation(number);
        public bool OnReceiveRepositoryID(Catalog.DataBase.ImitationGame specifies) => GetRepositoryID(specifies);
        public Catalog.XingAPI.Specify[] OnReceiveStrategy(long index) => GetStrategy(index);
        public Catalog.XingAPI.Specify[] GetUserStrategy() => GetCatalog(GetBestStrategyRecommend(Information.Statistics));
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
        public bool GetDuplicateResults(Models.ImitationGames game)
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
            return GetDuplicateResults(game, now.ToString(date));
        }
        public string GetDate(string code)
        {
            if (DateTime.TryParseExact(SetDate(code).Substring(0, 12), format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
                return string.Concat(date.ToLongDateString(), " ", date.ToShortTimeString());

            return string.Empty;
        }
        public bool SetStatisticalStorage()
        {
            if (Secrets.Memorizes.Count > 15)
            {
                var result = SetStatisticalStorage(Secrets.Memorizes);

                if (result)
                    Secrets.Memorizes.Clear();

                return result;
            }
            return true;
        }
        public Models.ImitationGames GetImitationModel(Catalog.DataBase.ImitationGame game) => new Models.ImitationGames
        {
            Assets = game.Assets,
            Code = game.Code,
            Commission = game.Commission,
            MarginRate = game.MarginRate,
            Strategy = game.Strategy,
            RollOver = game.RollOver,
            BaseTime = game.BaseTime,
            BaseShort = game.BaseShort,
            BaseLong = game.BaseLong,
            NonaTime = game.NonaTime,
            NonaShort = game.NonaShort,
            NonaLong = game.NonaLong,
            OctaTime = game.OctaTime,
            OctaShort = game.OctaShort,
            OctaLong = game.OctaLong,
            HeptaTime = game.HeptaTime,
            HeptaShort = game.HeptaShort,
            HeptaLong = game.HeptaLong,
            HexaTime = game.HexaTime,
            HexaShort = game.HexaShort,
            HexaLong = game.HexaLong,
            PentaTime = game.PentaTime,
            PentaShort = game.PentaShort,
            PentaLong = game.PentaLong,
            QuadTime = game.QuadTime,
            QuadShort = game.QuadShort,
            QuadLong = game.QuadLong,
            TriTime = game.TriTime,
            TriShort = game.TriShort,
            TriLong = game.TriLong,
            DuoTime = game.DuoTime,
            DuoShort = game.DuoShort,
            DuoLong = game.DuoLong,
            MonoTime = game.MonoTime,
            MonoShort = game.MonoShort,
            MonoLong = game.MonoLong
        };
        protected override string GetConvertCode(string code)
        {
            if (Code.Substring(0, 3).Equals(code.Substring(0, 3)) && Code.Substring(5).Equals(code.Substring(3)))
                return Code;

            return code;
        }
        public int SetIdentify(Setting setting) => SetIndentify(setting);
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
        internal static Catalog.XingAPI.Specify[] GetCatalog(Models.ImitationGames find)
        {
            var temp = new Catalog.XingAPI.Specify[10];
            int i = 0;

            while (i < temp.Length)
                switch (i)
                {
                    case 0:
                        temp[i++] = new Catalog.XingAPI.Specify
                        {
                            Assets = (ulong)find.Assets,
                            Code = find.Code,
                            Commission = find.Commission,
                            MarginRate = find.MarginRate,
                            Strategy = find.Strategy,
                            RollOver = find.RollOver,
                            Time = (uint)find.BaseTime,
                            Short = find.BaseShort,
                            Long = find.BaseLong
                        };
                        break;

                    case 1:
                        temp[i++] = new Catalog.XingAPI.Specify
                        {
                            Assets = (ulong)find.Assets,
                            Code = find.Code,
                            Commission = find.Commission,
                            MarginRate = find.MarginRate,
                            Strategy = find.Strategy,
                            RollOver = find.RollOver,
                            Time = (uint)find.NonaTime,
                            Short = find.NonaShort,
                            Long = find.NonaLong
                        };
                        break;

                    case 2:
                        temp[i++] = new Catalog.XingAPI.Specify
                        {
                            Assets = (ulong)find.Assets,
                            Code = find.Code,
                            Commission = find.Commission,
                            MarginRate = find.MarginRate,
                            Strategy = find.Strategy,
                            RollOver = find.RollOver,
                            Time = (uint)find.OctaTime,
                            Short = find.OctaShort,
                            Long = find.OctaLong
                        };
                        break;

                    case 3:
                        temp[i++] = new Catalog.XingAPI.Specify
                        {
                            Assets = (ulong)find.Assets,
                            Code = find.Code,
                            Commission = find.Commission,
                            MarginRate = find.MarginRate,
                            Strategy = find.Strategy,
                            RollOver = find.RollOver,
                            Time = (uint)find.HeptaTime,
                            Short = find.HeptaShort,
                            Long = find.HeptaLong
                        };
                        break;

                    case 4:
                        temp[i++] = new Catalog.XingAPI.Specify
                        {
                            Assets = (ulong)find.Assets,
                            Code = find.Code,
                            Commission = find.Commission,
                            MarginRate = find.MarginRate,
                            Strategy = find.Strategy,
                            RollOver = find.RollOver,
                            Time = (uint)find.HexaTime,
                            Short = find.HexaShort,
                            Long = find.HexaLong
                        };
                        break;

                    case 5:
                        temp[i++] = new Catalog.XingAPI.Specify
                        {
                            Assets = (ulong)find.Assets,
                            Code = find.Code,
                            Commission = find.Commission,
                            MarginRate = find.MarginRate,
                            Strategy = find.Strategy,
                            RollOver = find.RollOver,
                            Time = (uint)find.PentaTime,
                            Short = find.PentaShort,
                            Long = find.PentaLong
                        };
                        break;

                    case 6:
                        temp[i++] = new Catalog.XingAPI.Specify
                        {
                            Assets = (ulong)find.Assets,
                            Code = find.Code,
                            Commission = find.Commission,
                            MarginRate = find.MarginRate,
                            Strategy = find.Strategy,
                            RollOver = find.RollOver,
                            Time = (uint)find.QuadTime,
                            Short = find.QuadShort,
                            Long = find.QuadLong
                        };
                        break;

                    case 7:
                        temp[i++] = new Catalog.XingAPI.Specify
                        {
                            Assets = (ulong)find.Assets,
                            Code = find.Code,
                            Commission = find.Commission,
                            MarginRate = find.MarginRate,
                            Strategy = find.Strategy,
                            RollOver = find.RollOver,
                            Time = (uint)find.TriTime,
                            Short = find.TriShort,
                            Long = find.TriLong
                        };
                        break;

                    case 8:
                        temp[i++] = new Catalog.XingAPI.Specify
                        {
                            Assets = (ulong)find.Assets,
                            Code = find.Code,
                            Commission = find.Commission,
                            MarginRate = find.MarginRate,
                            Strategy = find.Strategy,
                            RollOver = find.RollOver,
                            Time = (uint)find.DuoTime,
                            Short = find.DuoShort,
                            Long = find.DuoLong
                        };
                        break;

                    case 9:
                        temp[i++] = new Catalog.XingAPI.Specify
                        {
                            Assets = (ulong)find.Assets,
                            Code = find.Code,
                            Commission = find.Commission,
                            MarginRate = find.MarginRate,
                            Strategy = find.Strategy,
                            RollOver = find.RollOver,
                            Time = (uint)find.MonoTime,
                            Short = find.MonoShort,
                            Long = find.MonoLong
                        };
                        break;
                }
            return temp;
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