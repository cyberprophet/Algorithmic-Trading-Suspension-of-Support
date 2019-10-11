using System;
using System.Windows.Forms;

namespace ShareInvest.Identify
{
    public partial class Confirm : Form
    {
        public static Confirm Get()
        {
            if (cf == null)
                cf = new Confirm();

            return cf;
        }
        public bool CheckCurrent()
        {
            return checkBox.Checked;
        }
        private void OnReceiveIdentify(object sender, EventHandler.Identify e)
        {
            checkBox.Text = string.Concat(DateTime.Now.ToString("H시 m분 s초  "), e.Confirm);
        }
        private Confirm()
        {
            InitializeComponent();

            Futures.Get().SendConfirm += OnReceiveIdentify;
        }
        private void Confirm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cf.Dispose();
            Dispose();
            Environment.Exit(0);
        }
        private static Confirm cf;
    }
}