using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.EventHandler;
using ShareInvest.Message;

namespace ShareInvest.Controls
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
            if (e.Confirm != null && e.Confirm.Equals(message))
            {
                if (TimerBox.Show(string.Concat(message, new Message().Continue), "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 35752).Equals((DialogResult)6))
                {

                }
                SendTab?.Invoke(this, new Mining(9));

                return;
            }
            BeginInvoke(new Action(() => checkBox.Text = e.Confirm != null ? string.Concat(DateTime.Now.ToString("H시 m분 s초\n"), e.Confirm) : string.Concat(e.Remaining, ".")));
            SendTab?.Invoke(this, new Mining(2));
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
        private const string message = "The latest Data Collection is Complete.";
        private readonly ConnectAPI api;
        private static ConfirmOrder cf;
        public event EventHandler<Mining> SendTab;
    }
}