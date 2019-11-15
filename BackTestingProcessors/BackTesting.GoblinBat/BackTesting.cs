using System;
using System.Windows.Forms;
using ShareInvest.BackTesting.SettingsScreen;
using ShareInvest.Market;

namespace ShareInvest.BackTesting
{
    public partial class BackTesting : Form
    {
        public BackTesting()
        {
            InitializeComponent();
            StartingPoint(new GoblinBatScreen(), new Progress());
            Dispose();
            Environment.Exit(0);
        }
        private void StartingPoint(GoblinBatScreen gs, Progress pro)
        {
            splitContainer.Panel1.Controls.Add(gs);
            splitContainer.Panel2.Controls.Add(pro);
            Width = gs.Width + 4;
            Height = gs.Height + pro.Height + 1;
            pro.Width = gs.Width;
            splitContainer.SplitterDistance = gs.Height;
            CenterToScreen();
            gs.SetProgress(pro);
            gs.SendMarket += OnReceiveMarket;
            ShowDialog();
        }
        private void OnReceiveMarket(object sender, OpenMarket e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(0);
            }
        }
    }
}