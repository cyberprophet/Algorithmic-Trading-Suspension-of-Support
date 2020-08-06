using System;
using System.Collections.Generic;
using System.Linq;
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
        public SendSecuritiesAPI(Tuple<int, string, int, int, string> order) => Convey = order;
        public SendSecuritiesAPI(string code, string name, string retention, string price) => Convey = new Tuple<string, string, string, string>(code, name, retention, price);
        public SendSecuritiesAPI(Tuple<string[], string[], string[], string[]> tuple) => Convey = tuple;
        public SendSecuritiesAPI(string code, Stack<string> stack) => Convey = new Tuple<string, Stack<string>>(code, stack);
        public SendSecuritiesAPI(Tuple<string, string, string> tuple)
        {
            if (tuple.Item1.StartsWith("106") && tuple.Item1.Length == 8)
                convert[tuple.Item1] = tuple.Item2;

            Convey = tuple;
        }
        public SendSecuritiesAPI(Tuple<string, string> tuple)
        {
            var temp = tuple.Item2.Split('F')[0].Trim();
            convert[tuple.Item1] = string.IsNullOrEmpty(temp) ? tuple.Item2 : temp;
            Convey = new Tuple<string, string, string>(tuple.Item1, tuple.Item2, string.Empty);
        }
        public SendSecuritiesAPI(FormWindowState state, Control accounts)
        {
            Convey = state;
            Accounts = accounts;
        }
        public SendSecuritiesAPI(Stack<StringBuilder> stack)
        {
            var dic = new Dictionary<string, Tuple<string, string>>();

            while (stack.Count > 0)
            {
                string[] temp = stack.Pop().ToString().Split(';'), name = temp[3].Split('_');
                string key = convert.FirstOrDefault(o => o.Value.Equals(name[name.Length - 1])).Key, exists = string.Empty;

                switch (temp[4])
                {
                    case "미래에셋대우":
                        exists = "미래대우";
                        break;

                    case "LG디스플레이":
                        exists = "LGD";
                        break;

                    case "SK머티리얼즈":
                        exists = "SK머티리얼";
                        break;

                    case "셀트리온헬스케어":
                        exists = "셀트리온헬";
                        break;

                    case "한화솔루션":
                        exists = "한화솔루션";
                        break;

                    case "SK이노베이션":
                        exists = "SK이노베이";
                        break;

                    default:
                        if (string.IsNullOrEmpty(key) == false)
                            dic[key] = new Tuple<string, string>(name[name.Length - 1], temp[5]);

                        else if (temp[4].Equals("KOSPI200"))
                            dic[convert.First(o => o.Key.StartsWith("101") && o.Key.Length == 8 && o.Key.EndsWith("000")).Key] = new Tuple<string, string>(temp[4], temp[5]);

                        else if (temp[4].Equals("코스닥150"))
                            dic[convert.First(o => o.Key.StartsWith("106") && o.Key.Length == 8 && o.Key.EndsWith("000")).Key] = new Tuple<string, string>(temp[4], temp[5]);

                        else
                        {
                            key = convert.FirstOrDefault(o => temp[4].StartsWith(o.Value, StringComparison.CurrentCultureIgnoreCase)).Key;

                            if (string.IsNullOrEmpty(key) == false)
                                dic[key] = new Tuple<string, string>(temp[4], temp[5]);
                        }
                        continue;
                }
                dic[convert.First(o => o.Value.Equals(exists)).Key] = new Tuple<string, string>(temp[4], temp[5]);
            }
            Convey = dic;
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

            else if (param[3].Length > 0 && param[3].Substring(0, 1).Equals("A") && double.TryParse(param[12]?.Insert(6, "."), out double ratio) && long.TryParse(param[11], out long valuation) && int.TryParse(param[6], out int reserve) && uint.TryParse(param[8], out uint purchase) && uint.TryParse(param[7], out uint current))
                Convey = new Tuple<string, string, int, dynamic, dynamic, long, double>(param[3].Substring(1).Trim(), param[4].Trim(), reserve, purchase, current, valuation, ratio);
        }
        public SendSecuritiesAPI(string message) => Convey = message;
        public SendSecuritiesAPI(int gubun, int status) => Convey = new Tuple<byte, byte>((byte)gubun, (byte)status);
        public SendSecuritiesAPI(long available) => Convey = available;
        public SendSecuritiesAPI(short error) => Convey = error;
        public SendSecuritiesAPI(Tuple<string, string, int, dynamic, dynamic, long, double> tuple)
        {
            if (tuple.Item1.Equals(tuple.Item2))
                Convey = new Tuple<string, string, int, dynamic, dynamic, long, double>(tuple.Item1, convert[tuple.Item1], tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7);

            else
                Convey = tuple;
        }
        static readonly Dictionary<string, string> convert = new Dictionary<string, string>();
    }
}