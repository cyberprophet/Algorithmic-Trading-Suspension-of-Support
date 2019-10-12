using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareInvest.BackTest
{
    public class Storage
    {
        public Storage(int type)
        {
            list = new List<Storage>();
            analysis = new Dictionary<string, string>();
            this.type = type > 0 ? @"\Statistics\Kosdaq150\" : @"\Statistics\Kospi200\";

            foreach (string val in new Enumerate(type))
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
                analysis[val.date] = val.open > 0 ? string.Concat((val.open - val.se).ToString("N4"), ",", (val.se - val.le).ToString("N4")) : val.commission;
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

                sb.Append(kv.Value);

                Statistics(sb);
            }
            Statistics(sb_analysis);
        }
        private void Statistics(StringBuilder sb)
        {
            string path = string.Concat(Environment.CurrentDirectory, type), file = DateTime.Now.ToString("yyMMdd") + ".csv";

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
            if (arr.Length > 5)
            {
                open = double.Parse(arr[5]);
                se = double.Parse(arr[6]);
                le = double.Parse(arr[7]);
            }
            date = arr[1];
            revenue = arr[3];
            cumulative = arr[4];
            strategy = arr[0];
            commission = arr[2];
        }
        private readonly double open;
        private readonly double se;
        private readonly double le;
        private readonly string commission;
        private readonly string revenue;
        private readonly string cumulative;
        private readonly string type;
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