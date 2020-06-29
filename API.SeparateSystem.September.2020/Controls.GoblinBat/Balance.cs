using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareInvest.Controls
{
    public partial class Balance : UserControl
    {
        public Balance(Catalog.AccountInformation info)
        {
            InitializeComponent();
            textIdentity.Text = info.Identity;
            textAccount.Text = (info.Account.Length == 11 ? info.Account : info.Account.Insert(4, "-")).Insert(9, "-");
            textName.Text = info.Name;
            textServer.Text = info.Server.ToString();
            textServer.ForeColor = info.Server ? Color.Navy : Color.Maroon;

            foreach (Control control in panel.Controls)
                if (control is TextBox box)
                {
                    box.MouseLeave += OnResponseToMouse;
                    box.MouseUp += OnResponseToMouse;
                }
        }
        public void OnReceiveDeposit(Tuple<long, long> param)
        {
            textAssets.Text = param.Item1.ToString("C0");
            textAvailable.Text = param.Item2.ToString("C0");
            textAssets.ForeColor = param.Item1 == 0 ? Color.Snow : param.Item1 > 0 ? Color.Maroon : Color.DeepSkyBlue;
            textAvailable.ForeColor = param.Item2 == 0 ? Color.Snow : param.Item2 > 0 ? Color.Maroon : Color.DeepSkyBlue;
        }
        public void OnReceiveMessage(string message) => labelMessage.Text = string.Concat(DateTime.Now.ToLongTimeString(), " ", message);
        void OnResponseToMouse(object sender, EventArgs e)
        {
            if (sender is TextBox box)
                switch (e.GetType().Name)
                {
                    case mouseEventArgs:
                        if (box.Enabled)
                            box.Enabled = false;

                        return;

                    case eventArgs:
                        if (box.Enabled == false)
                            box.Enabled = true;

                        return;
                }
        }
    }
}