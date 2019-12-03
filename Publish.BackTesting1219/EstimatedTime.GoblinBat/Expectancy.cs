using System;
using System.IO;
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
                string[] temp;
                int date = 0;
                DateTime max = new DateTime(1, 1, 1, 1, 1, 1), min = new DateTime(9999, 9, 9, 9, 9, 9);

                foreach (string val in Directory.GetDirectories(path))
                {
                    temp = val.Split('\\');
                    int recent = int.Parse(temp[temp.Length - 1]);

                    if (recent > date)
                        date = recent;
                }
                temp = Directory.GetFiles(string.Concat(path, date), "*.csv", SearchOption.TopDirectoryOnly);

                foreach (string str in temp)
                {
                    DateTime dt = new FileInfo(str).CreationTime;

                    if (DateTime.Compare(max, dt) < 0)
                        max = dt;

                    if (DateTime.Compare(dt, min) < 0)
                        min = dt;
                }
                return (int)(temp.Length / max.Subtract(min).TotalMinutes);
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Exception", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nThe First Connection is on by Default.\n\nEstimated Time is not Accurate."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return 155;
        }
    }
}