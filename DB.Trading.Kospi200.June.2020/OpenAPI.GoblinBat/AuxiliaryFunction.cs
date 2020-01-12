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
            new Opt50001(),
            new OPTKWFID()
        };
        protected readonly string[] exclude =
        {
            "115960",
            "006800",
            "001880",
            "072770"
        };
    }
}