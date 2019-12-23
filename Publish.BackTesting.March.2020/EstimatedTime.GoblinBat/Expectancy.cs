using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Log.Message;

namespace ShareInvest.EstimatedTime
{
    public class Expectancy
    {
        public int EstimatedTime(string path)
        {
            try
            {
                string[] temp, directory = Directory.GetDirectories(path);
                int date = 0;
                int[] total = new int[directory.Length];
                DateTime max = new DateTime(1, 1, 1, 1, 1, 1), min = new DateTime(9999, 9, 9, 9, 9, 9);

                foreach (string val in directory)
                {
                    temp = Directory.GetFiles(val, "*.csv", SearchOption.TopDirectoryOnly);
                    Parallel.ForEach(temp, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = (int)(Environment.ProcessorCount * 1.5)
                    }, (str) =>
                    {
                        DateTime dt = new FileInfo(str).CreationTime;

                        if (DateTime.Compare(max, dt) < 0)
                            max = dt;

                        if (DateTime.Compare(dt, min) < 0)
                            min = dt;
                    });
                    total[date++] = (int)(temp.Length / max.Subtract(min).TotalMinutes);
                }
                return total.Sum() / total.Length;
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Exception", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nThe First Connection is on by Default.\n\nEstimated Time is not Accurate."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return 75;
        }
    }
}