using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Const;
using ShareInvest.Interface;
using ShareInvest.Log.Message;

namespace ShareInvest.RecallStatistics
{
    public class Recall : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            long recent = 0, count;
            string[] temp;
            string path = Path.Combine(Application.StartupPath, @"..\Statistics\");

            try
            {
                Parallel.ForEach(Directory.GetFiles(path, "*.csv", SearchOption.AllDirectories), new ParallelOptions
                {
                    MaxDegreeOfParallelism = (int)(Environment.ProcessorCount * 1.5)
                }, (val) =>
                {
                    temp = val.Split('\\');
                    temp = temp[temp.Length - 1].Split('.');
                    count = long.Parse(temp[0]);

                    if (count > recent)
                        recent = count;
                });
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Error", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
            using (StreamReader sr = new StreamReader(string.Concat(path, recent.ToString(), ".csv")))
            {
                List<string> list = new List<string>(256);

                if (sr != null)
                    while (sr.EndOfStream == false)
                        list.Add(sr.ReadLine());

                yield return list.Count - 2;

                foreach (IMakeUp val in mp)
                    yield return MakeUp(list, val);

                yield return string.Concat(list[1].Substring(0, 8), "^", list[list.Count - 2].Substring(0, 8));
            }
        }
        private IMakeUp MakeUp(List<string> list, IMakeUp ip)
        {
            string[] transitory, temp = list[0].Split(',');
            Count = ip.Turn;
            long[] count = new long[temp.Length - 1];
            int i;

            do
            {
                transitory = list[list.Count - Count].Split(',');
                Count--;

                for (i = 0; i < count.Length; i++)
                    count[i] += long.Parse(transitory[i + 1]);
            }
            while (Count > 1);

            for (i = 0; i < count.Length; i++)
                ip.DescendingSort[temp[i + 1]] = count[i];

            return ip;
        }
        private int Count
        {
            get; set;
        }
        private readonly IMakeUp[] mp =
        {
            new MakeUpCumulative(),
            new MakeUpRecentDate(),
            new MakeUpWeekly(),
            new MakeUpBiweekly(),
            new MakeUpMonthly(),
            new MakeUpFor3Months()
        };
    }
}