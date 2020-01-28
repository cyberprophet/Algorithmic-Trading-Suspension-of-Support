using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareInvest.GoblinBatContext;

namespace ShareInvest.CallUpDataBase
{
    public class CallUpGoblinBat
    {
        protected string GetRecentFuturesCode(bool register)
        {
            if (register == false)
                using (var db = new GoblinBatDbContext())
                {
                    var instance = SqlProviderServices.Instance;

                    return db.Codes.FirstOrDefault(code => code.Info.Equals(db.Codes.Where(o => o.Code.Substring(0, 3).Equals("101") && o.Code.Substring(5, 3).Equals("000")).Max(o => o.Info))).Code;
                }
            return string.Empty;
        }
        protected bool GetRegister()
        {
            using (var db = new GoblinBatDbContext())
            {
                return db.Logs.Any();
            }
        }
    }
}