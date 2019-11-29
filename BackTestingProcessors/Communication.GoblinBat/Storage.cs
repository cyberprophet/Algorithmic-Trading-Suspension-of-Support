using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShareInvest.Log.Message;

namespace ShareInvest.Communication
{
    public class Storage
    {
        public Storage(string path)
        {
            list = new List<Storage>(128);
            analysis = new Dictionary<string, long>();

            foreach (string val in new Enumerate())
            {
                string[] arr = val.Split(',');

                list.Add(new Storage(arr));
            }
            int i = 0, count = list.Count;
            string[] squence = new string[count];
            string[] total = new string[count];

            foreach (Storage val in list)
            {
                total[i] = (long.Parse(val.cumulative) + val.unrealized).ToString();
                squence[i++] = val.strategy;
                analysis[val.date] = val.unrealized;
            }
            GC.Collect();
            sb = new StringBuilder(128);
            sb_analysis = new StringBuilder(128);

            for (i = 0; i < count - 1; i++)
            {
                if (!squence[i].Equals(squence[i + 1]))
                {
                    sb.Append(',').Append(squence[i]);
                    sb_analysis.Append(',').Append(total[i]);
                }
                if (i == count - 2)
                {
                    sb.Append(',').Append(squence[i + 1]);
                    sb_analysis.Append(',').Append(total[i + 1]);
                }
            }
            Statistics(sb, path);

            foreach (KeyValuePair<string, long> kv in analysis)
            {
                sb = new StringBuilder(128);
                sb.Append(kv.Key).Append(',');

                foreach (Storage val in list.FindAll(o => o.date.Equals(kv.Key)))
                {
                    Application.DoEvents();

                    if (kv.Key.Equals(analysis.Last().Key))
                    {
                        sb.Append(long.Parse(val.revenue) + kv.Value).Append(',');

                        continue;
                    }
                    sb.Append(val.revenue).Append(',');
                }
                Statistics(sb, path);
            }
            Statistics(sb_analysis, path);
        }
        private void Statistics(StringBuilder sb, string path)
        {
            Application.DoEvents();

            try
            {
                DirectoryInfo di = new DirectoryInfo(string.Concat(Path.Combine(Application.StartupPath, @"..\"), @"\Statistics\"));

                if (di.Exists == false)
                    di.Create();

                using StreamWriter sw = new StreamWriter(path, true);
                sw.WriteLine(sb);
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Exception", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
        }
        private Storage(string[] arr)
        {
            date = arr[1];
            revenue = arr[3];
            cumulative = arr[4];
            strategy = arr[0];
            unrealized = long.Parse(arr[2]);
        }
        private readonly long unrealized;
        private readonly string revenue;
        private readonly string cumulative;
        private readonly string date;
        private readonly string strategy;
        private readonly List<Storage> list;
        private readonly Dictionary<string, long> analysis;
        private readonly StringBuilder sb;
        private readonly StringBuilder sb_analysis;
    }
}