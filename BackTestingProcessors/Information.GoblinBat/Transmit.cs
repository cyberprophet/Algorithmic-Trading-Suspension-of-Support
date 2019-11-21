using System;
using System.IO;
using System.Threading.Tasks;
using ShareInvest.Communication;

namespace ShareInvest.Information
{
    public class Transmit
    {
        private void Save(string path)
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(path);

                if (info.Exists == false)
                    info.Create();

                using StreamWriter sw = new StreamWriter(string.Concat(path, DateTime.Now.Ticks, ".csv"));
                sw.WriteLine(string.Concat(Account, ',', BasicAssets));
            }
            catch (Exception ex)
            {
                TimerBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", 3750);
                Environment.Exit(0);
            }
        }
        public Transmit(string account, long assets)
        {
            Account = account;
            BasicAssets = assets;
            new Task(() => Save(string.Concat(Path.Combine(Environment.CurrentDirectory, @"..\"), @"\BasicMaterial\BackTesting\"))).Start();
        }
        public string Account
        {
            get; private set;
        }
        public long BasicAssets
        {
            get; private set;
        }
    }
}