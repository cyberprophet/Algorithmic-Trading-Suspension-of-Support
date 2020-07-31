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
        internal async Task<Queue<Charts>> CallUpTheChartAsync(Codes codes)
        {
            var start = CodeStorage.LastOrDefault(o => o.Code.StartsWith(codes.Code.Substring(0, 3)) && o.Code.EndsWith(codes.Code.Substring(5)) && string.Compare(o.MaturityMarketCap.Length == 8 ? o.MaturityMarketCap.Substring(2) : o.MaturityMarketCap, codes.MaturityMarketCap.Length == 8 ? codes.MaturityMarketCap.Substring(2) : codes.MaturityMarketCap) < 0).MaturityMarketCap;
            var queue = new Queue<Charts>();

            if (string.IsNullOrEmpty(start) && DateTime.TryParseExact(codes.MaturityMarketCap, ConvertDateTime(codes.MaturityMarketCap.Length), CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
                start = date.AddYears(-3).ToString(ConvertDateTime(6));

            var enumerable = await client.GetContext(new Catalog.Request.Charts
            {
                Code = codes.Code,
                Start = start.Length == 8 ? start.Substring(2) : start,
                End = codes.MaturityMarketCap.Length == 8 ? codes.MaturityMarketCap.Substring(2) : codes.MaturityMarketCap
            });
            if (enumerable != null)
                foreach (var arg in enumerable)
                    queue.Enqueue(arg);
            
            return queue;
        }
        internal async Task<Queue<Charts>> CallUpTheChartAsync(string code)
        {
            if (code.Length == 8 && (code.StartsWith("106") || code.StartsWith("101")) && code.EndsWith("000"))
            {
                var first = CodeStorage.Where(o => o.Code.StartsWith(code.Substring(0, 3)) && o.Code.EndsWith(code.Substring(5))).OrderBy(o => o.MaturityMarketCap.Length == 8 ? o.MaturityMarketCap.Substring(2) : o.MaturityMarketCap).First().MaturityMarketCap;
                code = CodeStorage.First(o => o.MaturityMarketCap.Equals(first) && o.Code.StartsWith(code.Substring(0, 3)) && o.Code.EndsWith(code.Substring(5))).Code;
            }
            var queue = new Queue<Charts>();
            var enumerable = await client.GetContext(new Catalog.Request.Charts
            {
                Code = code,
                Start = string.Empty,
                End = string.Empty
            });
            if (enumerable != null)
                foreach (var arg in enumerable)
                    queue.Enqueue(arg);

            return queue;
        }
        internal Temporary(int length)
        {
            client = GoblinBatClient.GetInstance();

            if (CodeStorage == null)
                CallUpTheCodesAsync(length).Wait();
        }
        async Task CallUpTheCodesAsync(int length) => CodeStorage = (await client.GetContext(new Codes { }, length) as List<Codes>)?.ToHashSet();
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
        readonly GoblinBatClient client;
        const string sFormat = "yyMMdd";
        const string lFormat = "yyyyMMdd";
    }
}