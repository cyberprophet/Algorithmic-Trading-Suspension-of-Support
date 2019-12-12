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
        public int ShortDayPeriod
        {
            get; private set;
        }
        public int LongDayPeriod
        {
            get; private set;
        }
        public int ShortTickPeriod
        {
            get; private set;
        }
        public int LongTickPeriod
        {
            get; private set;
        }
        public int Reaction
        {
            get; private set;
        }
        public int HedgeType
        {
            get; private set;
        }
        public string AccNo
        {
            get; private set;
        }
        public long BasicAssets
        {
            get; private set;
        }
        public BasicMaterial(IAccount account, IStatistics statistics)
        {
            new Task(() => Save(string.Concat(Path.Combine(Application.StartupPath, @"..\"), @"\BasicMaterial\Trading\"))).Start();
            AccNo = account.AccNo;
            BasicAssets = account.BasicAssets;
            ShortDayPeriod = statistics.ShortDayPeriod;
            ShortTickPeriod = statistics.ShortTickPeriod;
            LongDayPeriod = statistics.LongDayPeriod;
            LongTickPeriod = statistics.LongTickPeriod;
            Reaction = statistics.Reaction;
            HedgeType = statistics.HedgeType;
        }
        private void Save(string path)
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(path);

                if (info.Exists == false)
                    info.Create();

                using StreamWriter sw = new StreamWriter(string.Concat(path, DateTime.Now.Ticks, ".csv"));
                sw.WriteLine(string.Concat(AccNo, ',', BasicAssets, ',', ShortDayPeriod, ',', ShortTickPeriod, ',', LongDayPeriod, ',', LongTickPeriod, ',', Reaction, ',', HedgeType));
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