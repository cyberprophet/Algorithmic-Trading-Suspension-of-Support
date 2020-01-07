using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.EventHandler;
using ShareInvest.Log.Message;

namespace ShareInvest.OpenAPI
{
    public class Temporary : DistinctDate
    {
        public Temporary()
        {
            memo = new List<string>(32768);
            ConnectAPI.Get().SendMemorize += OnReceiveMemorize;
        }
        private void OnReceiveMemorize(object sender, Memorize e)
        {
            if (e.SPrevNext != null)
            {
                new Task(() => Save(e.Code, GetDistinctDate(CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now.AddDays(1 - DateTime.Now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1))).Start();

                return;
            }
            if (memo.Count > 0)
            {
                memo.Insert(0, string.Concat(e.Date, ",", e.Price, ",", e.Volume));

                return;
            }
            memo.Add(string.Concat(e.Date, ",", e.Price, ",", e.Volume));
        }
        private void Save(string code, string append)
        {
            string path = code.Substring(0, 3).Equals("101") ? Path.Combine(Application.StartupPath, @"..\Chart\") : string.Concat(Path.Combine(Application.StartupPath, @"..\Chart\"), append, @"\");

            try
            {
                DirectoryInfo di = new DirectoryInfo(path);

                if (di.Exists == false)
                    di.Create();

                using StreamWriter sw = new StreamWriter(string.Concat(path, code.Substring(0, 3).Equals("101") ? "Tick" : code, ".csv"), true);
                foreach (string val in memo)
                    if (val.Length > 0)
                        sw.WriteLine(val);

                memo.Clear();
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Error", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
        }
        private readonly List<string> memo;
    }
}