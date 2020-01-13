using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
        protected void SetStorage(string code, string[] param)
        {
            if (param.Length < 3)
                return;

            new Task(() =>
            {
                using (var db = new GoblinBatDbContext())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    string date = string.Empty;
                    int i, count = 0;
                    IList model = new List<Stocks>(32);

                    if (code.Contains("101") && code.Length > 6)
                        model = new List<Futures>(32);

                    else if (code.Length > 6)
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
                        switch (code.Length)
                        {
                            case 6:
                                model.Add(new Stocks
                                {
                                    Code = code,
                                    Date = long.Parse(string.Concat(temp[0], count.ToString("D2"))),
                                    Price = int.Parse(temp[1]),
                                    Volume = int.Parse(temp[2])
                                });
                                break;

                            case 8:
                                if (code.Contains("101"))
                                {
                                    model.Add(new Futures
                                    {
                                        Code = code,
                                        Date = long.Parse(string.Concat(temp[0], count.ToString("D2"))),
                                        Price = double.Parse(temp[1]),
                                        Volume = int.Parse(temp[2])
                                    });
                                    continue;
                                }
                                model.Add(new Options
                                {
                                    Code = code,
                                    Date = long.Parse(string.Concat(temp[0], count.ToString("D2"))),
                                    Price = double.Parse(temp[1]),
                                    Volume = int.Parse(temp[2])
                                });
                                break;
                        }
                    }
                    if (code.Length == 6)
                        db.Stocks.AddRange((List<Stocks>)model);

                    else if (code.Contains("101") && code.Length > 6)
                        db.Futures.AddRange((List<Futures>)model);

                    else
                        db.Options.AddRange((List<Options>)model);

                    db.SaveChanges();
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
                }
                catch (Exception ex)
                {
                    max = ex.Message.Length;
                }
            }
            return max.ToString().Length == 14 ? max.ToString().Substring(0, 12) : "DoesNotExist";
        }
        protected List<string> RequestCodeList(List<string> list)
        {
            using (var db = new GoblinBatDbContext())
            {
                Parallel.ForEach(db.Codes.ToList(), (temp) =>
                {
                    if (temp.Code.Length < 7 || (temp.Code.Length == 8 && DateTime.Compare(DateTime.ParseExact(temp.Info, "yyyyMMdd", null), DateTime.Now) >= 0))
                        list.Add(temp.Code);
                });
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