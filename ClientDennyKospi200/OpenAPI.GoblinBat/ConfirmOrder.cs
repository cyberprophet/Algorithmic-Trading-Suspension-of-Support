using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI
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
        private ConfirmOrder()
        {
            InitializeComponent();
            api = ConnectAPI.Get();
            api.SendConfirm += OnReceiveIdentify;
        }
        private void OnReceiveIdentify(object sender, Identify e)
        {
            checkBox.Text = e.Confirm != null ? string.Concat(DateTime.Now.ToString("H시 m분 s초"), e.Confirm) : string.Concat(e.Remaining, ".");
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
        private readonly ConnectAPI api;
        private static ConfirmOrder cf;
    }
}