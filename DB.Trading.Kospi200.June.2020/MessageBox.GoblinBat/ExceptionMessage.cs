using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShareInvest.Models;

namespace ShareInvest.Message
{
    public class ExceptionMessage
    {
        public ExceptionMessage(string message, string code)
        {
            this.code = code;
            new Task(() => Record(message)).Start();
        }
        void Record(string message)
        {
            try
            {
                var path = Path.Combine(Application.StartupPath, @"Message\");
                var di = new DirectoryInfo(path);
                var date = int.Parse(DateTime.Now.AddDays(-30).ToString("yyMMdd"));

                if (di.Exists)
                    foreach (var file in Directory.GetFiles(path))
                    {
                        string[] recent = file.Split('\\');
                        recent = recent[recent.Length - 1].Split('.');

                        if (date < int.Parse(recent[recent.Length - 2]))
                            continue;

                        new FileInfo(file).Delete();
                    }
                else
                    di.Create();

                using (var sw = new StreamWriter(string.Concat(path, DateTime.Now.ToString("yyMMdd"), ".txt"), true))
                {
                    if (code != null)
                        sw.WriteLine(code);

                    sw.WriteLine(DateTime.Now.ToShortTimeString());
                    sw.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Record(ex.StackTrace);
            }
        }
        public ExceptionMessage(ImitationGames games) => Record(new StringBuilder(where).Append(games.Assets).Append(nCode).Append(games.Code).Append(commission).Append(games.Commission).Append(rate).Append(games.MarginRate).Append(strategy).Append(games.Strategy).Append(over).Append(games.RollOver).Append(bs).Append(games.BaseShort).Append(bl).Append(games.BaseLong).Append(nt).Append(games.NonaTime).Append(ns).Append(games.NonaShort).Append(nl).Append(games.NonaLong).Append(ot).Append(games.OctaTime).Append(os).Append(games.OctaShort).Append(ol).Append(games.OctaLong).Append(ht).Append(games.HeptaTime).Append(hs).Append(games.HeptaShort).Append(hl).Append(games.HeptaLong).Append(xt).Append(games.HexaTime).Append(xs).Append(games.HexaShort).Append(xl).Append(games.HexaLong).Append(pt).Append(games.PentaTime).Append(ps).Append(games.PentaShort).Append(pl).Append(games.PentaLong).Append(qt).Append(games.QuadTime).Append(qs).Append(games.QuadShort).Append(ql).Append(games.QuadLong).Append(tt).Append(games.TriTime).Append(ts).Append(games.TriShort).Append(tl).Append(games.TriLong).Append(dt).Append(games.DuoTime).Append(ds).Append(games.DuoShort).Append(dl).Append(games.DuoLong).Append(mt).Append(games.MonoTime).Append(ms).Append(games.MonoShort).Append(ml).Append(games.MonoLong).Append(date).ToString());
        public ExceptionMessage(string message) => new Task(() => Record(message)).Start();
        readonly string code;
        const string date = " order by Date desc";
        const string mt = " and MonoTime=";
        const string ms = " and MonoShort=";
        const string ml = " and MonoLong=";
        const string dt = " and DuoTime=";
        const string ds = " and DuoShort=";
        const string dl = " and DuoLong=";
        const string tt = " and TriTime=";
        const string ts = " and TriShort=";
        const string tl = " and TriLong=";
        const string qt = " and QuadTime=";
        const string qs = " and QuadShort=";
        const string ql = " and QuadLong=";
        const string pt = " and PentaTime=";
        const string ps = " and PentaShort=";
        const string pl = " and PentaLong=";
        const string xt = " and HexaTime=";
        const string xs = " and HexaShort=";
        const string xl = " and HexaLong=";
        const string ht = " and HeptaTime=";
        const string hs = " and HeptaShort=";
        const string hl = " and HeptaLong=";
        const string ot = " and OctaTime=";
        const string os = " and OctaShort=";
        const string ol = " and OctaLong=";
        const string nt = " and NonaTime=";
        const string ns = " and NonaShort=";
        const string nl = " and NonaLong=";
        const string bs = " and BaseShort=";
        const string bl = " and BaseLong=";
        const string over = " and RollOver=";
        const string strategy = " and Strategy=";
        const string rate = " and MarginRate=";
        const string where = "where Assets=";
        const string nCode = " and Code=";
        const string commission = " and Commission=";
    }
}