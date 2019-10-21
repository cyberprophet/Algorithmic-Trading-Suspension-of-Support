using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareInvest.BackTest
{
    public class Storage
    {
        public Storage()
        {
            list = new List<Storage>();
            analysis = new Dictionary<string, string>();

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
                total[i] = val.cumulative;
                squence[i++] = val.strategy;
                analysis[val.date] = val.commission;
            }
            sb = new StringBuilder();
            sb_analysis = new StringBuilder();

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
            Statistics(sb);

            foreach (KeyValuePair<string, string> kv in analysis)
            {
                sb = new StringBuilder();
                sb.Append(kv.Key).Append(',');

                foreach (Storage val in list.FindAll(o => o.date.Equals(kv.Key)))
                    sb.Append(val.revenue).Append(',');

                Statistics(sb);
            }
            Statistics(sb_analysis);
        }
        private void Statistics(StringBuilder sb)
        {
            string dt = DateTime.Now.Hour > 23 || DateTime.Now.Hour < 9 ? DateTime.Now.AddDays(-1).ToString("yyMMdd") : DateTime.Now.ToString("yyMMdd"), path = string.Concat(Environment.CurrentDirectory, @"\Statistics\"), file = dt + ".csv";

            try
            {
                di = new DirectoryInfo(path);

                if (di.Exists == false)
                    di.Create();

                using (sw = new StreamWriter(path + file, true))
                {
                    sw.WriteLine(sb);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private Storage(string[] arr)
        {
            date = arr[1];
            revenue = arr[3];
            cumulative = arr[4];
            strategy = arr[0];
            commission = arr[2];
        }
        private readonly string commission;
        private readonly string revenue;
        private readonly string cumulative;
        private readonly string date;
        private readonly string strategy;
        private readonly List<Storage> list;
        private readonly Dictionary<string, string> analysis;
        private readonly StringBuilder sb;
        private readonly StringBuilder sb_analysis;
        private DirectoryInfo di;
        private StreamWriter sw;
    }
}