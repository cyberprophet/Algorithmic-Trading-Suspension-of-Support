using System;
using System.Collections;
using System.Linq;
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
                    if (db.Codes.Where(o => o.Code.Equals(code)).Any() == false)
                    {
                        db.Codes.Add(new Codes
                        {
                            Code = code,
                            Name = name,
                            Info = info
                        });
                        db.SaveChanges();
                    }
                }
            }).Start();
        }
        protected readonly IEnumerable[] catalog =
        {
            new Opt50001()
        };
    }
}