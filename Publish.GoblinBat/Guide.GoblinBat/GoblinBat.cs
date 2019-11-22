using System;
using System.IO;
using System.Windows.Forms;
using IWshRuntimeLibrary;

namespace ShareInvest.Guide
{
    public partial class GoblinBat : UserControl
    {
        public GoblinBat()
        {
            InitializeComponent();
            SetRoute(new WshShell());
        }
        private void SetRoute(WshShell ws)
        {
            string[] temp;
            IWshShortcut sc;

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