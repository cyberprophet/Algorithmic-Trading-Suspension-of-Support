using System;
using System.Windows.Forms;
using ShareInvest.AutoMessageBox;
using ShareInvest.Controls;
using ShareInvest.EventHandler;
using ShareInvest.Publish;

namespace ShareInvest.SelfScalping
{
    public partial class SelfScalping : Form
    {
        public SelfScalping()
        {
            InitializeComponent();
            StartProgress();
        }
        private void StartProgress()
        {
            using (ChooseSelectButton sb = new ChooseSelectButton(PublicFutures.Get()))
            {
                panel.Controls.Add(sb);
                sb.Dock = DockStyle.Fill;
                StartPosition = FormStartPosition.CenterScreen;
                sb.SendQuit += OnReceiveDialogClose;
                ShowDialog();
            }
            Environment.Exit(0);
        }
        private void OnReceiveDialogClose(object sender, ForceQuit e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                Box.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", 3750);
                Dispose();
                Environment.Exit(0);
            }
        }
    }
}