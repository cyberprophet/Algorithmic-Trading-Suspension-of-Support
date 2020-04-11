using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Message;
using ShareInvest.Models;

namespace ShareInvest.GoblinBatContext
{
    public class CallUp
    {
        protected async Task<List<string>> RequestCodeListAsync(List<string> list)
        {
            string code = string.Empty;

            try
            {
                using (var db = new GoblinBatDbContext(key))
                    foreach (var temp in db.Codes.Select(o => new
                    {
                        o.Code
                    }))
                    {
                        code = temp.Code;

                        if (db.Codes.Any(o => o.Code.Length < 6))
                            await db.Codes.BulkDeleteAsync(db.Codes.Where(o => o.Code.Length < 6), o => o.BatchSize = 100).ConfigureAwait(false);

                        if (db.Days.Any(o => o.Code.Equals(temp.Code) && o.Date < 10000000))
                        {
                            await db.Days.BulkDeleteAsync(db.Days.Where(o => o.Date < 10000000), o => o.BatchSize = 100).ConfigureAwait(false);

                            if (db.Days.Any(o => o.Code.Length < 6))
                                await db.Days.BulkDeleteAsync(db.Days.Where(o => o.Code.Length < 6), o => o.BatchSize = 100).ConfigureAwait(false);
                        }
                        if (temp.Code.Length == 6 && (db.Days.Any(o => o.Code.Equals(temp.Code)) == false || db.Stocks.Any(o => o.Code.Equals(temp.Code)) == false || int.Parse(db.Days.Where(o => o.Code.Equals(temp.Code)).Max(o => o.Date).ToString().Substring(2)) < int.Parse(db.Stocks.Where(o => o.Code.Equals(code)).Min(o => o.Date).ToString().Substring(0, 6))))
                        {
                            list.Add(temp.Code);

                            if (db.Stocks.Any(o => o.Code.Length < 6))
                                await db.Stocks.BulkDeleteAsync(db.Stocks.Where(o => o.Code.Length < 6), o => o.BatchSize = 100).ConfigureAwait(false);
                        }
                        else if (temp.Code.Length == 8 && temp.Code.Substring(5, 3).Equals("000") && db.Futures.Any(o => o.Date < 100000000000000))
                        {
                            await db.Futures.BulkDeleteAsync(db.Futures.Where(o => o.Date < 100000000000000), o => o.BatchSize = 100).ConfigureAwait(false);

                            if (db.Futures.Any(o => o.Code.Length < 8))
                                await db.Futures.BulkDeleteAsync(db.Futures.Where(o => o.Code.Length < 8), o => o.BatchSize = 100).ConfigureAwait(false);
                        }
                        else if (temp.Code.Length == 8 && temp.Code.Substring(5, 3).Equals("000") == false && db.Options.Any(o => o.Date < 100000000000000))
                        {
                            await db.Options.BulkDeleteAsync(db.Options.Where(o => o.Date < 100000000000000), o => o.BatchSize = 100).ConfigureAwait(false);

                            if (db.Options.Any(o => o.Code.Length < 8))
                                await db.Options.BulkDeleteAsync(db.Options.Where(o => o.Code.Length < 8), o => o.BatchSize = 100).ConfigureAwait(false);
                        }
                    }
            }
            catch (InvalidOperationException ex)
            {
                new ExceptionMessage(ex.StackTrace, code);
            }
            catch (Exception ex)
            {
                using (var db = new GoblinBatDbContext(key))
                {
                    var stocks = db.Stocks.Where(o => o.Code.Equals(code));

                    if (stocks.Any(o => o.Date < 100000000000000))
                        await db.Stocks.BulkDeleteAsync(stocks.Where(o => o.Date < 100000000000000), o => o.BatchSize = 100).ConfigureAwait(false);
                }
                new ExceptionMessage(ex.StackTrace, code);
            }
            return list;
        }
        protected List<string> RequestCodeList(List<string> list, string[] market)
        {
            try
            {
                using (var db = new GoblinBatDbContext(key))
                {
                    foreach (var temp in db.Codes.Select(o => new
                    {
                        o.Code,
                        o.Info
                    }))
                        if (temp.Code.Length == 6 && Array.Exists(market, o => o.Equals(temp.Code)) || temp.Code.Length == 8 && DateTime.Compare(DateTime.ParseExact(temp.Info, "yyyyMMdd", null), DateTime.Now) >= 0)
                            list.Add(temp.Code);
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace);
            }
            return list;
        }
        protected string GetRetention(int param, string code)
        {
            long max = 0;

            try
            {
                using (var db = new GoblinBatDbContext(key))
                {
                    switch (param)
                    {
                        case 1:
                            max = db.Futures.Where(o => o.Code.Equals(code)).Max(o => o.Date);
                            break;

                        case 2:
                            max = db.Options.Where(o => o.Code.Equals(code)).Max(o => o.Date);
                            break;

                        case 3:
                            max = db.Stocks.Where(o => o.Code.Equals(code)).Max(o => o.Date);
                            break;

                        case 4:
                            max = db.Days.Where(o => o.Code.Equals(code)).Max(o => o.Date);
                            break;
                    };
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex.TargetSite.Name.Equals("GetValue") == false)
                    new ExceptionMessage(ex.TargetSite.Name, code);
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, code);
            }
            return max > 0 ? (max.ToString().Length > 12 ? max.ToString().Substring(0, 12) : max.ToString()) : "DoesNotExist";
        }
        protected async void SetInsertCode(string code, string name, string info)
        {
            try
            {
                using (var db = new GoblinBatDbContext(key))
                {
                    if (db.Codes.Where(o => o.Code.Equals(code) && o.Info.Equals(info) && o.Name.Equals(name)).Any())
                        return;

                    db.Codes.AddOrUpdate(new Codes
                    {
                        Code = code,
                        Name = name,
                        Info = info
                    });
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, code);
            }
        }
        protected async void SetStorage(string code, string[] param)
        {
            if (param.Length < 3)
                return;

            try
            {
                string date = string.Empty;
                int i, count = 0;
                bool days = param[0].Split(',')[0].Length == 8, stocks = code.Length == 6, futures = code.Length > 6 && code.Substring(5, 3).Equals("000"), options = code.Length > 6 && !code.Substring(5, 3).Equals("000");
                IList model;

                if (futures)
                    model = new List<Futures>(32);

                else if (options)
                    model = new List<Options>(32);

                else if (days)
                    model = new List<Days>(32);

                else
                    model = new List<Stocks>(32);

                for (i = param.Length - 2; i > -1; i--)
                {
                    var temp = param[i].Split(',');

                    if (temp[0].Length == 8)
                    {
                        model.Add(new Days
                        {
                            Code = code,
                            Date = int.Parse(temp[0]),
                            Price = double.Parse(temp[1])
                        });
                        continue;
                    }
                    else if (temp[0].Equals(date))
                        count++;

                    else
                    {
                        date = temp[0];
                        count = 0;
                    }
                    if (stocks)
                        model.Add(new Stocks
                        {
                            Code = code,
                            Date = long.Parse(string.Concat(temp[0], count.ToString("D3"))),
                            Price = int.Parse(temp[1]),
                            Volume = int.Parse(temp[2])
                        });
                    else if (options)
                        model.Add(new Options
                        {
                            Code = code,
                            Date = long.Parse(string.Concat(temp[0], count.ToString("D3"))),
                            Price = double.Parse(temp[1]),
                            Volume = int.Parse(temp[2])
                        });
                    else if (futures)
                        model.Add(new Futures
                        {
                            Code = code,
                            Date = long.Parse(string.Concat(temp[0], count.ToString("D3"))),
                            Price = double.Parse(temp[1]),
                            Volume = int.Parse(temp[2])
                        });
                }
                using (var db = new GoblinBatDbContext(key))
                {
                    db.Configuration.AutoDetectChangesEnabled = true;

                    if (days)
                        await db.BulkInsertAsync((List<Days>)model, o =>
                        {
                            o.InsertIfNotExists = true;
                            o.BatchSize = 10000;
                            o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                            o.AutoMapOutputDirection = false;
                        });
                    else if (stocks)
                        await db.BulkInsertAsync((List<Stocks>)model, o =>
                        {
                            o.InsertIfNotExists = true;
                            o.BatchSize = 10000;
                            o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                            o.AutoMapOutputDirection = false;
                        });
                    else if (options)
                        await db.BulkInsertAsync((List<Options>)model, o =>
                        {
                            o.InsertIfNotExists = true;
                            o.BatchSize = 10000;
                            o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                            o.AutoMapOutputDirection = false;
                        });
                    else if (futures)
                        await db.BulkInsertAsync((List<Futures>)model, o =>
                        {
                            o.InsertIfNotExists = true;
                            o.BatchSize = 10000;
                            o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                            o.AutoMapOutputDirection = false;
                        });
                    db.Configuration.AutoDetectChangesEnabled = false;
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, code);
            }
        }
        protected void SetStorage(string code, StringBuilder sb)
        {
            string onTime = string.Empty, date = DateTime.Now.ToString("yyMMdd"), yesterday = DateTime.Now.AddDays(-1).ToString("yyMMdd");
            int count = 0;
            var external = new Secret().GetTrustedConnection(key);
            var dic = new Dictionary<string, string>();
            var model = new List<Datum>();
            var record = new Queue<string>();

            foreach (var str in sb.ToString().Split('*'))
                if (str.Length > 0)
                {
                    var temp = str.Split(';');

                    if (temp[0].Length == 6 && double.TryParse(temp[1].Split(',')[0], out double price) && price > 105.95)
                    {
                        if (temp[0].Equals(onTime))
                            count++;

                        else
                        {
                            onTime = temp[0];
                            count = 0;
                        }
                        if (uint.TryParse(onTime, out uint today))
                        {
                            var time = string.Concat(today > 175959 ? yesterday : date, onTime, count.ToString("D3"));

                            if (dic.ContainsKey(time) && ulong.TryParse(time, out ulong relate))
                            {
                                foreach (var kv in dic.OrderBy(o => o.Key))
                                    if (time.Substring(0, 12).Equals(kv.Key.Substring(0, 12)))
                                        relate += 1;

                                dic[relate.ToString()] = temp[1];
                                count = 0;

                                continue;
                            }
                            dic[time] = temp[1];
                        }
                    }
                }
            switch (external)
            {
                case true:
                    foreach (var kv in dic)
                    {
                        var temp = kv.Value.Split(',');

                        if (temp.Length == 2)
                        {
                            model.Add(new Datum
                            {
                                Code = code,
                                Date = kv.Key,
                                Price = temp[0],
                                Volume = temp[1]
                            });
                            continue;
                        }
                        model.Add(new Datum
                        {
                            Code = code,
                            Date = kv.Key,
                            SellPrice = temp[0],
                            SellQuantity = temp[1],
                            TotalSellAmount = temp[2],
                            BuyPrice = temp[3],
                            BuyQuantity = temp[4],
                            TotalBuyAmount = temp[5]
                        });
                    }
                    SetStorage(model);
                    return;

                case false:
                    foreach (var kv in dic.OrderBy(o => o.Key))
                        record.Enqueue(string.Concat(kv.Key, ",", kv.Value));

                    SetStorage(code, record);
                    return;
            }
        }
        async void SetStorage(List<Datum> model)
        {
            try
            {
                using (var db = new GoblinBatDbContext(key))
                {
                    db.Configuration.AutoDetectChangesEnabled = true;
                    await db.BulkInsertAsync(model, o =>
                    {
                        o.InsertIfNotExists = true;
                        o.BatchSize = 15000;
                        o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                        o.AutoMapOutputDirection = false;
                    });
                    db.Configuration.AutoDetectChangesEnabled = false;
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, ex.TargetSite.Name);
            }
        }
        void SetStorage(string code, Queue<string> model)
        {
            var path = Path.Combine(Application.StartupPath, code);

            try
            {
                var directory = new DirectoryInfo(path);

                if (directory.Exists == false)
                    directory.Create();

                using (var sw = new StreamWriter(Path.Combine(path, string.Concat(DateTime.Now.ToString(date), extension)), true))
                    while (model.Count > 0)
                    {
                        var str = model.Dequeue();

                        if (str.Length > 0)
                            sw.WriteLine(str);
                    }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, ex.TargetSite.Name);
            }
        }
        protected CallUp(string key) => this.key = key;
        readonly string key;
        const string extension = ".csv";
        const string date = "yyMMdd";
        const string q3 = "101Q3000";
    }
}