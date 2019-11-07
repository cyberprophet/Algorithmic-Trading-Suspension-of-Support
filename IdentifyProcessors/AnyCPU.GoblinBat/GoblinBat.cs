using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ShareInvest.AutoMessageBox;
using ShareInvest.Control;
using ShareInvest.EventHandler;
using ShareInvest.SelectableMessageBox;

namespace ShareInvest.AnyCPU
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            InitializeComponent();
            Activate((int)Choose.Show("Please Select the Button You Want to Proceed. . .", "ShareInvest GoblinBat Kospi200 TradingSystem", "Trading", "BackTest", "Exit"));
        }
        private void Activate(int result)
        {
            GetTermsAndConditions();
            SetVisibleCore(false);

            if (result.Equals(6))
                Process.Start(Array.Find(Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"), "*.exe", SearchOption.AllDirectories), o => o.Contains("Trading.exe")));

            else if (result.Equals(7))
                Process.Start(Array.Find(Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"), "*.exe", SearchOption.AllDirectories), o => o.Contains("BackTesting.exe")));

            Dispose();
            Environment.Exit(0);
        }
        private void GetTermsAndConditions()
        {
            using TermsConditions tc = new TermsConditions();
            Controls.Add(tc);
            tc.Dock = DockStyle.Fill;
            Size = tc.Size;
            StartPosition = FormStartPosition.CenterScreen;
            tc.SendQuit += OnReceiveDialogClose;
            ShowDialog();
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