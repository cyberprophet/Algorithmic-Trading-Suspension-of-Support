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
            Quotes quotes;

            if (code.Length > 6 && code.Substring(5, 3).Equals(futures))
                try
                {
                    using (var db = new GoblinBatDbContext(key))
                        foreach (var temp in db.Datums.Where(o => o.Code.Contains(code.Substring(0, 3))).Select(o => new
                        {
                            o.Date,
                            o.Price,
                            o.Volume,
                            o.SellPrice,
                            o.SellQuantity,
                            o.TotalSellAmount,
                            o.BuyPrice,
                            o.BuyQuantity,
                            o.TotalBuyAmount
                        }).OrderBy(o => o.Date))
                        {
                            if (uint.TryParse(temp.Date.Substring(0, 8), out uint date) && date < 20040318)
                                continue;

                            if (temp.Price == null && temp.Volume == null)
                                quotes = new Quotes
                                {
                                    Time = temp.Date,
                                    SellPrice = temp.SellPrice,
                                    SellQuantity = temp.SellQuantity,
                                    SellAmount = temp.TotalSellAmount,
                                    BuyPrice = temp.BuyPrice,
                                    BuyQuantity = temp.BuyQuantity,
                                    BuyAmount = temp.TotalBuyAmount
                                };
                            else
                                quotes = new Quotes
                                {
                                    Time = temp.Date,
                                    Price = temp.Price,
                                    Volume = temp.Volume
                                };
                            chart.Enqueue(quotes);
                        }
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace, code);
                }
            return chart;
        }
        protected string GetStrategy()
        {
            try
            {
                using (var db = new GoblinBatDbContext(key))
                    return db.Codes.First(c => c.Info.Equals(db.Codes.Where(o => o.Code.Length == 8 && o.Code.Substring(0, 3).Equals(kospi200f) && o.Code.Substring(5, 3).Equals(futures)).Max(o => o.Info))).Code;
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return string.Empty;
        }
        protected Stack<Specify[]> GetStrategy(string code)
        {
            var stack = new Stack<Specify[]>();

            try
            {
                using (var db = new GoblinBatDbContext(key))
                {

                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, code);
            }
            return stack;
        }
        protected CallUpStatisticalAnalysis(string key) : base(key) => this.key = key;
        readonly string key;
    }
}