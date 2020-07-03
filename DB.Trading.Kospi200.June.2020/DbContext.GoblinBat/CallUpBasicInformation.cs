using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ShareInvest.Catalog.XingAPI;
using ShareInvest.Message;
using ShareInvest.Models;

namespace ShareInvest.GoblinBatContext
{
    public class CallUpBasicInformation
    {
        protected (IOrderedEnumerable<KeyValuePair<string, Queue<Catalog.OpenAPI.Chart>>>, int) GetBasicChart(string code, string date)
        {
            try
            {
                string path = Path.Combine(Application.StartupPath, chart, code), procedure = string.Empty;
                var check = long.MinValue;
                var result = int.MaxValue;
                var charts = new Queue<Catalog.OpenAPI.Chart>();
                var directory = new DirectoryInfo(path);
                var temp = new Dictionary<string, Queue<Catalog.OpenAPI.Chart>>();

                if (directory.Exists)
                {
                    foreach (var file in Directory.GetFiles(path, "*.res", SearchOption.TopDirectoryOnly))
                        if (file.Contains(basic) == false)
                            using (var sr = new StreamReader(file))
                            {
                                if (sr != null)
                                    while (sr.EndOfStream == false)
                                    {
                                        var str = sr.ReadLine().Split(',');

                                        if (long.TryParse(str[0], out long time) && int.TryParse(str[1], out int price))
                                        {
                                            charts.Enqueue(new Catalog.OpenAPI.Chart
                                            {
                                                Date = time,
                                                Price = price
                                            });
                                            check = time;
                                        }
                                        if (string.IsNullOrEmpty(procedure))
                                            procedure = str[0].Substring(0, 6);
                                    }
                                temp[procedure] = new Queue<Catalog.OpenAPI.Chart>(charts);
                                charts.Clear();
                                procedure = string.Empty;
                            }
                    var sTemp = new Dictionary<long, int>(32);
                    using (var db = new GoblinBatDbContext(key))
                        foreach (var stock in db.Stocks.Where(o => o.Code.Equals(code) && o.Date > check).AsNoTracking().Select(o => new { o.Date, o.Price }))
                            sTemp[stock.Date] = stock.Price;

                    foreach (var kv in sTemp.OrderBy(o => o.Key))
                    {
                        charts.Enqueue(new Catalog.OpenAPI.Chart
                        {
                            Date = kv.Key,
                            Price = kv.Value
                        });
                        if (string.IsNullOrEmpty(procedure))
                            procedure = kv.Key.ToString().Substring(0, 6);
                    }
                    if (string.IsNullOrEmpty(procedure) == false)
                    {
                        temp[procedure] = new Queue<Catalog.OpenAPI.Chart>(charts);
                        result = string.Compare(procedure, date);
                        using (var sw = new StreamWriter(string.Concat(path, @"\", procedure.Substring(0, 2), res), true))
                            while (charts.Count > 0)
                            {
                                var str = charts.Dequeue();
                                sw.WriteLine(string.Concat(str.Date, ',', str.Price));
                            }
                        if (result <= 0)
                        {
                            var copy = new Queue<string>();
                            using (var sr = new StreamReader(string.Concat(path, basic)))
                                if (sr != null)
                                    while (sr.EndOfStream == false)
                                    {
                                        var str = sr.ReadLine();

                                        if (str.Split(',')[0].Substring(2).Equals(procedure))
                                            break;

                                        copy.Enqueue(str);
                                    }
                            using (var sw = new StreamWriter(string.Concat(path, basic)))
                                while (copy.Count > 0)
                                    sw.WriteLine(copy.Dequeue());
                        }
                    }
                }
                return (temp.OrderBy(o => o.Key), result);
            }
            catch (Exception ex)
            {
                if (DateTime.TryParseExact(date, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime infoDate))
                    new ExceptionMessage(ex.StackTrace, string.Concat(code, " ", infoDate.ToShortDateString()));

                else
                    new ExceptionMessage(ex.StackTrace);
            }
            return (null, int.MinValue);
        }
        protected (string, Queue<Catalog.OpenAPI.Chart>) GetBasicChart(string code)
        {
            string path = Path.Combine(Application.StartupPath, chart, code), date = string.Empty;
            var charts = new Queue<Catalog.OpenAPI.Chart>();
            var directory = new DirectoryInfo(path);
            var check = preferred.Equals(code);

            try
            {
                if (check)
                    directory.Delete(check);

                if (directory.Exists)
                    using (var sr = new StreamReader(string.Concat(path, basic)))
                    {
                        if (sr != null)
                            while (sr.EndOfStream == false)
                            {
                                var str = sr.ReadLine().Split(',');
                                date = str[0].Substring(2);

                                if (long.TryParse(str[0], out long time) && int.TryParse(str[1], out int price))
                                    charts.Enqueue(new Catalog.OpenAPI.Chart
                                    {
                                        Date = time,
                                        Price = price
                                    });
                            }
                    }
                else
                {
                    using (var db = new GoblinBatDbContext(key))
                        foreach (var dp in db.Days.Where(o => o.Code.Equals(code)).AsNoTracking().OrderBy(o => o.Date).Select(o => new { o.Date, o.Price }))
                        {
                            charts.Enqueue(new Catalog.OpenAPI.Chart
                            {
                                Date = dp.Date,
                                Price = dp.Price
                            });
                            date = dp.Date.ToString().Substring(2);
                        }
                    directory.Create();
                    using (var sw = new StreamWriter(string.Concat(path, basic), true))
                        foreach (var str in charts)
                            sw.WriteLine(string.Concat(str.Date, ',', str.Price));
                }
            }
            catch (Exception ex)
            {
                new ExceptionMessage(ex.StackTrace, code);
            }
            return (date, charts);
        }
        protected Stack<double> GetBasicChart(string check, DateTime now, Specify specify, int period, Stack<double> stack)
        {
            if (specify.Time == 1440)
                try
                {
                    for (int i = 0; i > int.MinValue; i--)
                    {
                        var date = i == 0 ? now.ToString(format) : now.AddDays(i).ToString(format);
                        int time;

                        if (check.Equals(date))
                            using (var db = new GoblinBatDbContext(key))
                            {
                                switch (now.Hour)
                                {
                                    case 6:
                                    case 7:
                                    case 8:
                                        time = db.Charts.Any(o => o.Time == 660) ? 660 : 405;
                                        break;

                                    case 16:
                                    case 17:
                                        time = 405;
                                        break;

                                    default:
                                        return stack;
                                }
                                var call = db.Charts.Where(o => o.Code.Equals(specify.Code) && o.Time == time && o.Base == period).AsNoTracking();

                                if (call.Any(o => o.Date.Equals(check)))
                                {
                                    foreach (var e in call.OrderByDescending(o => o.Date).Take(period + 1).Select(o => new { o.Value, o.Date }).OrderBy(o => o.Date))
                                        stack.Push(e.Value);

                                    return stack;
                                }
                            }
                    }
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace, specify.Code);
                }
            return null;
        }
        protected Stack<double> GetBasicChart(Stack<double> stack, Specify specify, int period)
        {
            var path = Path.Combine(Application.StartupPath, chart, specify.Code, specify.Time.ToString());
            var exists = new DirectoryInfo(path);
            var file = string.Concat(path, @"\", period, res);
            var files = new FileInfo(file);

            if (exists.Exists && files.Exists)
                using (var sr = new StreamReader(file))
                    try
                    {
                        if (sr != null)
                            while (sr.EndOfStream == false)
                                if (double.TryParse(sr.ReadLine().Split(',')[1], out double value))
                                    stack.Push(value);
                    }
                    catch (Exception ex)
                    {
                        new ExceptionMessage(ex.StackTrace, specify.Strategy);
                    }
            else
            {
                var time = (int)specify.Time;
                var temp = new Dictionary<string, double>();
                using (var db = new GoblinBatDbContext(key))
                    try
                    {
                        var call = db.Charts.Where(o => o.Code.Equals(specify.Code) && o.Time == time && o.Base == period).AsNoTracking();

                        if (call.Any(o => o.Date.Equals(time == 1440 ? rDate : rTime)))
                            foreach (var e in call.OrderByDescending(o => o.Date).Take(period + 1).Select(o => new
                            {
                                o.Date,
                                o.Value
                            }).OrderBy(o => o.Date))
                            {
                                stack.Push(e.Value);
                                temp[e.Date] = e.Value;
                            }
                    }
                    catch (Exception ex)
                    {
                        new ExceptionMessage(ex.StackTrace, specify.Strategy);
                    }
                if (temp.Count > 0)
                    try
                    {
                        if (exists.Exists == false)
                            exists.Create();

                        using (var sw = new StreamWriter(file))
                            foreach (var kv in temp.OrderBy(o => o.Key))
                                sw.WriteLine(string.Concat(kv.Key, ',', kv.Value));
                    }
                    catch (Exception ex)
                    {
                        new ExceptionMessage(ex.StackTrace, specify.Strategy);
                    }
            }
            return stack;
        }
        protected bool SetBasicChart(Queue<Charts> charts)
        {
            bool result;
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    db.BulkInsert(charts, o =>
                    {
                        o.InsertIfNotExists = true;
                        o.BatchSize = 15000;
                        o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                        o.AutoMapOutputDirection = false;
                    });
                    result = true;
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                    result = false;
                }
                finally
                {
                    db.Configuration.AutoDetectChangesEnabled = true;
                }
            return result;
        }
        protected CallUpBasicInformation(string key) => this.key = key;
        readonly string key;
        const string format = "yyMMdd";
        const string res = ".res";
        const string chart = "Chart";
        const string rDate = "200403";
        const string rTime = "2004031545";
        const string basic = @"\Basic.res";
        const string preferred = "005935";
    }
}