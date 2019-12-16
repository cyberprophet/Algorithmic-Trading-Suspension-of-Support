using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Communication;
using ShareInvest.Log.Message;

namespace ShareInvest.AssetManagement
{
    public class Asset : IAsset
    {
        public string Account
        {
            get; private set;
        }
        public long Assets
        {
            get; private set;
        }
        public int Hedge
        {
            get; private set;
        }
        public int Reaction
        {
            get; private set;
        }
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
        public int Base
        {
            get; private set;
        }
        public int Sigma
        {
            get; private set;
        }
        public int Percent
        {
            get; private set;
        }
        public int Max
        {
            get; private set;
        }
        public int Quantity
        {
            get; private set;
        }
        public int Time
        {
            get; private set;
        }
        public string[] Temp
        {
            get; private set;
        }
        public Asset()
        {
            Temp = GetStrategyUsed();
            Account = Temp[0];
            Assets = long.Parse(Temp[1]);
            ShortDayPeriod = int.Parse(Temp[3]);
            ShortTickPeriod = int.Parse(Temp[2]);
            LongDayPeriod = int.Parse(Temp[5]);
            LongTickPeriod = int.Parse(Temp[4]);
            Reaction = int.Parse(Temp[6]);
            Hedge = int.Parse(Temp[7]);
            Base = int.Parse(Temp[8]);
            Sigma = int.Parse(Temp[9]);
            Percent = int.Parse(Temp[10]);
            Max = int.Parse(Temp[11]);
            Quantity = int.Parse(Temp[12]);
            Time = int.Parse(Temp[13]);
        }
        private string[] GetStrategyUsed()
        {
            string[] temp;
            ulong recent = 0;
            string assets = string.Empty;

            try
            {
                Parallel.ForEach(Directory.GetFiles(string.Concat(Path.Combine(Application.StartupPath, @"..\"), @"\BasicMaterial\Trading\"), "*.csv", SearchOption.AllDirectories), (val) =>
                {
                    temp = val.Split('\\');
                    temp = temp[temp.Length - 1].Split('.');
                    ulong count = ulong.Parse(temp[0]);

                    if (count > recent)
                        recent = count;
                });
                using StreamReader sr = new StreamReader(string.Concat(Path.Combine(Application.StartupPath, @"..\"), @"\BasicMaterial\Trading\", recent, ".csv"));
                if (sr != null)
                    while (sr.EndOfStream == false)
                        assets = sr.ReadLine();
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Exception", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
            return assets.Split(',');
        }
    }
}