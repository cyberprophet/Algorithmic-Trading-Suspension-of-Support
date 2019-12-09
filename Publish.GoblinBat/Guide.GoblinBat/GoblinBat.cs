using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using Microsoft.Win32;

namespace ShareInvest.Guide
{
    public partial class GoblinBat : UserControl
    {
        public GoblinBat()
        {
            InitializeComponent();
            SetRoute(new WshShell());
            webBrowser.Navigate(@"https://youtu.be/HhkZEPW1d3I");
        }
        private void SetRoute(WshShell ws)
        {
            IWshShortcut sc;
            string[] temp;
            string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", bat = "GoblinBat", key = Array.Find(Directory.GetFiles(Application.StartupPath, string.Concat(bat, ".exe"), SearchOption.AllDirectories), o => o.StartsWith(bat));

            try
            {
                foreach (string file in Directory.GetFiles(Environment.CurrentDirectory, "*.exe", SearchOption.AllDirectories))
                {
                    temp = file.Split('\\');
                    temp = temp[temp.Length - 1].Split('.');

                    if (Array.Exists(repeat, o => o.Equals(temp[0])))
                    {
                        sc = (IWshShortcut)ws.CreateShortcut(string.Concat(desk, @"\", temp[0], ".lnk"));
                        sc.TargetPath = file;
                        sc.Description = file.Contains(trading) ? "GoblinBatTraing" : "GoblinBatBackTesting";
                        sc.IconLocation = file.Replace("exe", "ico");
                        sc.Save();
                    }
                }
                if (Registry.LocalMachine.OpenSubKey(path).GetValue(bat) == null)
                    Registry.LocalMachine.OpenSubKey(path, true).SetValue(bat, key);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private readonly string[] repeat = { trading, backTesting };
        private readonly string desk = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private const string trading = "Kospi200";
        private const string backTesting = "BackTesting";
    }
}