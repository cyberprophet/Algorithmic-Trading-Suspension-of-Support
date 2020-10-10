using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using ShareInvest.Catalog;
using ShareInvest.Client;

namespace ShareInvest.Analysis
{
    class Temporary
    {
        internal static HashSet<Codes> CodeStorage
        {
            get; set;
        }
        internal static HashSet<uint> RemainingDay
        {
            get; set;
        }
        internal async Task<Queue<Charts>> CallUpTheChartAsync(Codes codes)
        {
            var start = CodeStorage.LastOrDefault(o => o.Code.StartsWith(codes.Code.Substring(0, 3)) && o.Code.EndsWith(codes.Code.Substring(5)) && string.Compare(o.MaturityMarketCap.Length == 8 ? o.MaturityMarketCap.Substring(2) : o.MaturityMarketCap, codes.MaturityMarketCap.Length == 8 ? codes.MaturityMarketCap.Substring(2) : codes.MaturityMarketCap) < 0).MaturityMarketCap;
            var queue = new Queue<Charts>();

            if (string.IsNullOrEmpty(start) && DateTime.TryParseExact(codes.MaturityMarketCap, ConvertDateTime(codes.MaturityMarketCap.Length), CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
                start = date.AddYears(-3).ToString(ConvertDateTime(6));

            if (await client.GetContext(new Catalog.Request.Charts
            {
                Code = codes.Code,
                Start = start.Length == 8 ? start.Substring(2) : start,
                End = codes.MaturityMarketCap.Length == 8 ? codes.MaturityMarketCap.Substring(2) : codes.MaturityMarketCap
            }) is IEnumerable<Charts> enumerable)
                foreach (var arg in enumerable)
                    queue.Enqueue(arg);

            return queue;
        }
        internal async Task<Queue<Charts>> CallUpTheChartAsync(string code)
        {
            if (code.Length == 8 && (code.StartsWith("106") || code.StartsWith("101")) && code.EndsWith("000"))
                code = CodeStorage.First(f => f.MaturityMarketCap.Equals(CodeStorage.Where(o => o.Code.Length == 8 && o.Code.StartsWith(code.Substring(0, 3)) && o.Code.EndsWith(code.Substring(5))).OrderBy(o => o.MaturityMarketCap.Length == 8 ? o.MaturityMarketCap.Substring(2) : o.MaturityMarketCap).First().MaturityMarketCap) && f.Code.StartsWith(code.Substring(0, 3)) && f.Code.EndsWith(code.Substring(5))).Code;

            var queue = new Queue<Charts>();

            if (await client.GetContext(new Catalog.Request.Charts
            {
                Code = code,
                Start = string.Empty,
                End = string.Empty
            }) is IEnumerable<Charts> enumerable)
                foreach (var arg in enumerable)
                    queue.Enqueue(arg);

            return queue;
        }
        internal async Task<Queue<Charts>> CallUpTheChartAsync(Catalog.Request.Charts charts)
        {
            var queue = new Queue<Charts>();

            if (await client.GetContext(charts) is IEnumerable<Charts> enumerable)
                foreach (var arg in enumerable)
                    queue.Enqueue(arg);

            return queue;
        }
        internal async Task<Queue<Catalog.Request.ConfirmRevisedStockPrice>> CallUpTheRevisedStockPrice(string code) => await client.GetContext(new Catalog.OpenAPI.RevisedStockPrice { Code = code });
        internal async Task<string> FindTheChartStartsAsync(string code) => await client.GetContext(new Catalog.Request.Charts { Code = code, Start = empty, End = empty }) as string;
        internal Temporary(int length)
        {
            if (client == null)
                client = GoblinBatClient.GetInstance();

            CallUpTheCodesAsync(length).Wait();
        }
        async Task CallUpTheCodesAsync(int length)
        {
            if (CodeStorage == null)
            {
                Length = length;
                CodeStorage = (await client.GetContext(new Codes { }, length) as List<Codes>)?.ToHashSet();

                if (length == 8)
                    RemainingDay = new HashSet<uint>() { 190910U, 191211U };
            }
            else if (Length < length)
                foreach (var code in client.GetContext(new Codes { }, length).Result as List<Codes>)
                    if (CodeStorage.Add(code) && RemainingDay == null)
                    {
                        Length = length;
                        RemainingDay = new HashSet<uint>() { 190910U, 191211U };
                    }
        }
        string ConvertDateTime(int length)
        {
            switch (length)
            {
                case 6:
                    return sFormat;

                case 8:
                    return lFormat;
            }
            return null;
        }
        static int Length
        {
            get; set;
        }
        readonly GoblinBatClient client;
        const string sFormat = "yyMMdd";
        const string lFormat = "yyyyMMdd";
        const string empty = "empty";
    }
}