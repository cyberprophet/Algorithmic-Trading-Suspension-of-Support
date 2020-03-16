using System;
using System.Collections.Generic;
using System.Linq;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.Message;

namespace ShareInvest.GoblinBatContext
{
    public class CallUpStatisticalAnalysis : CallUpGoblinBat
    {
        protected Queue<Quotes> GetQuotes(string code)
        {
            var chart = new Queue<Quotes>();

            if (code.Length > 6 && code.Substring(5, 3).Equals("000"))
            {
                using (var db = new GoblinBatDbContext())
                {
                    try
                    {
                        var tick = db.Quotes.Where(o => o.Code.Contains(code.Substring(0, 3))).Select(o => new
                        {
                            o.Date,
                            o.Contents
                        }).OrderBy(o => o.Date);

                        foreach (var temp in tick)
                        {
                            if (int.TryParse(temp.Date.Substring(0, 6), out int date) && date < 200315)
                                continue;

                            var contents = temp.Contents.Split(new char[] { '^', '*' });
                            Quotes quotes;

                            switch (contents.Length)
                            {
                                case 2:
                                    quotes = new Quotes
                                    {
                                        Time = temp.Date,
                                        Price = contents[0],
                                        Volume = contents[1]
                                    };
                                    break;

                                case 6:
                                    quotes = new Quotes
                                    {
                                        Time = temp.Date,
                                        SellPrice = contents[0],
                                        SellQuantity = contents[1],
                                        SellAmount = contents[2],
                                        BuyPrice = contents[3],
                                        BuyQuantity = contents[4],
                                        BuyAmount = contents[5]
                                    };
                                    break;
                            }
                            chart.Enqueue(quotes);
                        }
                        return chart;
                    }
                    catch (Exception ex)
                    {
                        new ExceptionMessage(ex.StackTrace, code);
                    }
                }
            }
            return null;
        }
    }
}