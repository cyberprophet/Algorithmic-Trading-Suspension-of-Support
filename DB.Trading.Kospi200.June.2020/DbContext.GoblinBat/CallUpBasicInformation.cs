using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
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
                        new ExceptionMessage(ex.StackTrace);
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
                        new ExceptionMessage(ex.StackTrace);
                    }
                if (temp.Count > 0)
                {
                    if (exists.Exists == false)
                        exists.Create();

                    using (var sw = new StreamWriter(file))
                        try
                        {
                            foreach (var kv in temp.OrderBy(o => o.Key))
                                sw.WriteLine(string.Concat(kv.Key, ',', kv.Value));
                        }
                        catch (Exception ex)
                        {
                            new ExceptionMessage(ex.StackTrace);
                        }
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
                        o.BatchSize = 3500;
                        o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                        o.AutoMapOutputDirection = false;
                    });
                    result = true;
                }
                catch (Exception ex)
                {
                    result = false;
                    new ExceptionMessage(ex.StackTrace);
                }
                finally
                {
                    db.Configuration.AutoDetectChangesEnabled = true;
                }
            return result;
        }
        protected CallUpBasicInformation(string key) => this.key = key;
        readonly string key;
        const string res = ".res";
        const string chart = "Chart";
        const string rDate = "200403";
        const string rTime = "2004031545";
    }
}