using System;
using System.Windows.Forms;
using ShareInvest.AssetManagement;
using ShareInvest.BackTesting.SettingsScreen;

namespace ShareInvest.BackTesting
{
    public partial class BackTesting : Form
    {
        public BackTesting(int count)
        {
            InitializeComponent();
            StartingPoint(new GoblinBatScreen(count, new Asset()), new Progress());
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
            ShowDialog();
        }
    }
}