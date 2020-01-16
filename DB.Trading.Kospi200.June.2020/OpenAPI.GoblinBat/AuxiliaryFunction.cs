using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareInvest.Catalog;
using ShareInvest.GoblinBatContext;
using ShareInvest.Models;

namespace ShareInvest.OpenAPI
{
    public class AuxiliaryFunction
    {
        protected string GetDistinctDate(int usWeekNumber)
        {
            DayOfWeek dt = DateTime.Now.AddDays(1 - DateTime.Now.Day).DayOfWeek;
            int check = dt.Equals(DayOfWeek.Friday) || dt.Equals(DayOfWeek.Saturday) ? 3 : 2;

            return usWeekNumber > check || usWeekNumber == check && (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday)) ? DateTime.Now.AddMonths(1).ToString("yyyyMM") : DateTime.Now.ToString("yyyyMM");
        }
        protected void SetStorage(string code, string[] param, string day)
        {
            IList model = new List<Days>(32);

            /*
            model.Add(new Days
                {
                    Code = code,
                    Date = int.Parse(temp[0]),
                    Price = double.Parse(temp[1])
                });

            else if (code.Contains("101") && code.Length > 6)
                    db.BulkInsert((List<Days>)model, o =>
                    {
                        o.InsertIfNotExists = true;
                        o.BatchSize = 10000;
                        o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                        o.AutoMapOutputDirection = false;
                    });
                */
        }        
        protected void SetStorage(string code, string[] param)
        {
            if (param.Length < 3)
                return;

            new Task(() =>
            {
                try
                {
                    using (var db = new GoblinBatDbContext())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        string date = string.Empty;
                        int i, count = 0;
                        bool stocks = code.Length == 6, futures = code.Length > 6 && code.Substring(5, 3).Equals("000"), options = code.Length > 6 && !code.Substring(5, 3).Equals("000");

                        IList model = new List<Stocks>(32);

                        if (futures)
                            model = new List<Futures>(32);

                        else if (options)
                            model = new List<Options>(32);

                        for (i = param.Length - 2; i > -1; i--)
                        {
                            var temp = param[i].Split(',');

                            if (temp[0].Equals(date))
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
                        db.Configuration.AutoDetectChangesEnabled = true;

                        if (stocks)
                            db.BulkInsert((List<Stocks>)model, o =>
                            {
                                o.InsertIfNotExists = true;
                                o.BatchSize = 10000;
                                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                                o.AutoMapOutputDirection = false;
                            });
                        else if (options)
                            db.BulkInsert((List<Options>)model, o =>
                            {
                                o.InsertIfNotExists = true;
                                o.BatchSize = 10000;
                                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                                o.AutoMapOutputDirection = false;
                            });
                        else if (futures)
                            db.BulkInsert((List<Futures>)model, o =>
                            {
                                o.InsertIfNotExists = true;
                                o.BatchSize = 10000;
                                o.SqlBulkCopyOptions = (int)SqlBulkCopyOptions.Default | (int)SqlBulkCopyOptions.TableLock;
                                o.AutoMapOutputDirection = false;
                            });
                    }
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.ToString());
                }
            }).Start();
        }
        protected void SetInsertCode(string code, string name, string info)
        {
            new Task(() =>
            {
                using (var db = new GoblinBatDbContext())
                {
                    if (db.Codes.Where(o => o.Code.Equals(code) && o.Info.Equals(info) && o.Name.Equals(name)).Any())
                        return;

                    db.Codes.AddOrUpdate(new Codes
                    {
                        Code = code,
                        Name = name,
                        Info = info
                    });
                    db.SaveChanges();
                }
            }).Start();
        }
        protected string Retention(int param, string code)
        {
            long max = 0;
            using (var db = new GoblinBatDbContext())
            {
                try
                {
                    switch (param)
                    {
                        case 0:
                            max = db.Futures.Where(o => o.Code.Equals(code)).Max(o => o.Date);
                            break;

                        case 1:
                            max = db.Options.Where(o => o.Code.Equals(code)).Max(o => o.Date);
                            break;

                        case 2:
                            max = db.Stocks.Where(o => o.Code.Equals(code)).Max(o => o.Date);
                            break;
                    };
                    return max > 0 ? max.ToString().Substring(0, 12) : "DoesNotExist";
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.ToString());
                }
            }
            return max > 0 ? max.ToString().Substring(0, 12) : "DoesNotExist";
        }
        protected List<string> RequestCodeList(List<string> list)
        {
            using (var db = new GoblinBatDbContext())
            {
                foreach (var temp in db.Codes.ToList())
                    if (temp.Code.Length < 7 || (temp.Code.Length == 8 && DateTime.Compare(DateTime.ParseExact(temp.Info, "yyyyMMdd", null), DateTime.Now) >= 0))
                        list.Add(temp.Code);
            }
            return list;
        }
        protected void FixUp(string[] info)
        {
            if (info[0].Length > 0)
                SetInsertCode(info[0], info[1], info[35]);
        }
        protected List<string> SetCodeStorage(string[] arr)
        {
            int i, l = arr.Length;
            StringBuilder sb = new StringBuilder(1024);
            List<string> inven = new List<string>(16);

            for (i = 0; i < l; i++)
            {
                if (arr[i].Length > 0)
                {
                    if (i % 100 < 99 && i != l - 2)
                    {
                        sb.Append(arr[i]).Append(";");

                        continue;
                    }
                    if (i % 100 == 99 || i == l - 2)
                    {
                        sb.Append(arr[i]);
                        inven.Add(sb.ToString().Trim());
                        sb = new StringBuilder(1024);

                        continue;
                    }
                }
            }
            return inven;
        }
        protected readonly IEnumerable[] catalog =
        {
            new Opt50028(),
            new Opt50066(),
            new Opt10079(),
            new Opt50001(),
            new OPTKWFID(),
            new Opt10081()
        };
        protected readonly string[] exclude =
        {
            "115960",
            "006800",
            "001880",
            "072770"
        };
        protected const string exists = "Information that already Exists";
    }
}