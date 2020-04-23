using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.Message;
using ShareInvest.Models;

namespace ShareInvest.GoblinBatContext
{
    public class CallUpBasicInformation
    {
        protected Stack<double> GetBasicChart(Stack<double> stack, Specify specify, int period)
        {
            using (var db = new GoblinBatDbContext(key))
                try
                {
                    var call = db.Charts.Where(o => o.Code.Equals(specify.Code) && o.Time == specify.Time && o.Base == period).AsNoTracking();

                    if (call.Any(o => o.Date.Equals(specify.Time == 1440 ? rDate : rTime)))
                        foreach (var e in call.OrderByDescending(o => o.Date).Take(period).Select(o => new
                        {
                            o.Date,
                            o.Value
                        }).OrderBy(o => o.Date))
                            stack.Push(e.Value);
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace);
                }
            return stack;
        }
        protected bool SetBasicChart(IEnumerable<Charts> charts)
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
        const string rDate = "200403";
        const string rTime = "2004031545";
    }
}