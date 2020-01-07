using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Interface;
using ShareInvest.Log.Message;

namespace ShareInvest.Basic
{
    public class BasicMaterial
    {
        public BasicMaterial(IAccount account, IStatistics statistics)
        {
            new Task(() => Save(Path.Combine(Application.StartupPath, @"..\BasicMaterial\Trading\"), string.Concat(account.AccNo, ',', account.BasicAssets, ',', statistics.ShortTickPeriod, ',', statistics.ShortDayPeriod, ',', statistics.LongTickPeriod, ',', statistics.LongDayPeriod, ',', statistics.Reaction, ',', statistics.HedgeType, ',', statistics.Base, ',', statistics.Sigma, ',', statistics.Percent, ',', statistics.Max, ',', statistics.Quantity, ',', statistics.Time))).Start();
        }
        private void Save(string path, string save)
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(path);

                if (info.Exists == false)
                    info.Create();

                using StreamWriter sw = new StreamWriter(string.Concat(path, DateTime.Now.Ticks, ".csv"));
                sw.WriteLine(save);
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Error", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
        }
    }
}