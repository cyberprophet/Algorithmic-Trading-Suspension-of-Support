using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.EventHandler;

namespace ShareInvest.Publish
{
    public partial class ConfirmOrder : UserControl
    {
        public static ConfirmOrder Get()
        {
            if (cf == null)
                cf = new ConfirmOrder();

            return cf;
        }
        public bool CheckCurrent()
        {
            return checkBox.Checked;
        }
        private void OnReceiveIdentify(object sender, Identify e)
        {
            checkBox.Text = e.Confirm != null ? string.Concat(DateTime.Now.ToString("H시 m분 s초  No."), e.Confirm) : string.Concat(e.Remaining, " Days.");
        }
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckCurrent())
            {
                checkBox.ForeColor = Color.Ivory;

                return;
            }
            api.OnReceiveBalance = true;
            checkBox.ForeColor = Color.Maroon;
        }
        private ConfirmOrder()
        {
            InitializeComponent();
            api = PublicFutures.Get();
            api.SendConfirm += OnReceiveIdentify;
        }
        private readonly PublicFutures api;
        private static ConfirmOrder cf;
    }
}