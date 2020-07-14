using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ShareInvest.EventHandler
{
    public class SendSecuritiesAPI : EventArgs
    {
        public object Convey
        {
            get; private set;
        }
        public Control Accounts
        {
            get; private set;
        }
        public SendSecuritiesAPI(Tuple<string, string> tuple) => convert[tuple.Item1] = tuple.Item2;
        public SendSecuritiesAPI(FormWindowState state, Control accounts)
        {
            Convey = state;
            Accounts = accounts;
        }
        public SendSecuritiesAPI(Control control, string account, string password)
        {
            Accounts = control;
            Convey = string.Concat(account.Replace("-", string.Empty), ";", password);
        }
        public SendSecuritiesAPI(string sEvaluation, string sDeposit, string sAvailable)
        {
            if (long.TryParse(sEvaluation, out long evaluation) && long.TryParse(sDeposit, out long deposit) && long.TryParse(sAvailable, out long available))
                Convey = new Tuple<long, long>(evaluation + deposit, available);
        }
        public SendSecuritiesAPI(string sDeposit, string sAvailable)
        {
            if (long.TryParse(sDeposit, out long deposit) && long.TryParse(sAvailable, out long available))
                Convey = new Tuple<long, long>(deposit, available);
        }
        public SendSecuritiesAPI(string[] param)
        {
            if (param[0].Length == 8 && int.TryParse(param[4], out int quantity) && double.TryParse(param[9], out double fRate) && long.TryParse(param[8], out long fValuation) && double.TryParse(param[6], out double fCurrent) && double.TryParse(param[5], out double fPurchase))
                Convey = new Tuple<string, string, int, dynamic, dynamic, long, double>(param[0], param[1].Equals(param[0]) ? convert[param[1]] : param[1], param[2].Equals("1") ? -quantity : quantity, fPurchase, fCurrent, fValuation, fRate * 0.01);

            else if (param[3].Substring(0, 1).Equals("A") && double.TryParse(param[12]?.Insert(6, "."), out double ratio) && long.TryParse(param[11], out long valuation) && int.TryParse(param[6], out int reserve) && uint.TryParse(param[8], out uint purchase) && uint.TryParse(param[7], out uint current))
                Convey = new Tuple<string, string, int, dynamic, dynamic, long, double>(param[3].Substring(1).Trim(), param[4].Trim(), reserve, purchase, current, valuation, ratio);
        }
        public SendSecuritiesAPI(long available) => Convey = available;
        public SendSecuritiesAPI(Tuple<string, string, int, dynamic, dynamic, long, double> tuple)
        {
            if (tuple.Item1.Equals(tuple.Item2))
                Convey = new Tuple<string, string, int, dynamic, dynamic, long, double>(tuple.Item1, convert[tuple.Item1], tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7);

            else
                Convey = tuple;
        }
        public SendSecuritiesAPI(int nCodeCount, StringBuilder sArrCode) => Convey = new Tuple<int, string>(nCodeCount, sArrCode.ToString());
        public SendSecuritiesAPI(string message) => Convey = message;
        static readonly Dictionary<string, string> convert = new Dictionary<string, string>();
    }
}