using System;
using System.IO;
using ShareInvest.Communication;

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
        public Asset()
        {
            string[] temp = ReadCSV().Split(',');
            Account = temp[0];
            Assets = long.Parse(temp[1]);
            ShortDayPeriod = int.Parse(temp[2]);
            ShortTickPeriod = int.Parse(temp[3]);
            LongDayPeriod = int.Parse(temp[4]);
            LongTickPeriod = int.Parse(temp[5]);
            Reaction = int.Parse(temp[6]);
            Hedge = int.Parse(temp[7]);
        }
        private string ReadCSV()
        {
            string[] temp;
            ulong recent = 0;
            string assets = string.Empty;

            try
            {
                foreach (string val in Directory.GetFiles(string.Concat(Path.Combine(Environment.CurrentDirectory, @"..\"), @"\BasicMaterial\Trading\"), "*.csv", SearchOption.AllDirectories))
                {
                    temp = val.Split('\\');
                    temp = temp[temp.Length - 1].Split('.');
                    ulong count = ulong.Parse(temp[0]);

                    if (count > recent)
                        recent = count;
                }
                using StreamReader sr = new StreamReader(string.Concat(Path.Combine(Environment.CurrentDirectory, @"..\"), @"\BasicMaterial\Trading\", recent, ".csv"));
                if (sr != null)
                    while (sr.EndOfStream == false)
                        assets = sr.ReadLine();
            }
            catch (Exception ex)
            {
                TimerBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", 3750);
                Environment.Exit(0);
            }
            return assets;
        }
    }
}