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
            analysis = new Dictionary<string, int>();

            foreach (string val in new Enumerate())
            {
                string[] arr = val.Split(',');

                Save(new Storage(arr[1], int.Parse(arr[2]), int.Parse(arr[3]), long.Parse(arr[4]), arr[0]));
            }
            int i = 0, count = list.Count;
            string[] squence = new string[count];
            long[] total = new long[count];

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

            foreach (KeyValuePair<string, int> kv in analysis)
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
            string path = Environment.CurrentDirectory + @"\Statistics\", file = DateTime.Now.ToString("yyMMdd") + ".csv";

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
        private void Save(Storage save)
        {
            list.Add(save);

            string path = Environment.CurrentDirectory + @"\Analysis\", file = save.date + ".csv";

            try
            {
                di = new DirectoryInfo(path);

                if (di.Exists == false)
                    di.Create();

                using (sw = new StreamWriter(path + file, true))
                {
                    sw.WriteLine(string.Concat(save.strategy, ',', save.revenue, ',', save.cumulative, ',', save.commission));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private Storage(string date, int commission, int revenue, long cumulative, string strategy)
        {
            this.date = date;
            this.commission = commission;
            this.revenue = revenue;
            this.cumulative = cumulative;
            this.strategy = strategy;
        }
        private readonly int revenue;
        private readonly int commission;
        private readonly long cumulative;
        private readonly string date;
        private readonly string strategy;
        private readonly List<Storage> list;
        private readonly Dictionary<string, int> analysis;
        private readonly StringBuilder sb;
        private readonly StringBuilder sb_analysis;
        private DirectoryInfo di;
        private StreamWriter sw;
    }
}