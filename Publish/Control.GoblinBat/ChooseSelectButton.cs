using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.AutoMessageBox;
using ShareInvest.Chart;
using ShareInvest.Const;
using ShareInvest.EventHandler;
using ShareInvest.Publish;

namespace ShareInvest.Controls
{
    public partial class ChooseSelectButton : UserControl
    {
        public ChooseSelectButton(PublicFutures api)
        {
            InitializeComponent();
            this.api = api;
            api.SetAPI(axAPI);
            GetChart();
            api.StartProgress(new SpecifyKospi200 { Division = false }, new FreeVersion());
            api.Send += OnReceiveRealData;
            api.SendExit += OnReceiveExit;
            new Temporary().Send += OnReceiveExit;
        }
        private double StopLossTick
        {
            get; set;
        } = 0.35;
        private double RevenueTick
        {
            get; set;
        } = 0.15;
        private void OnReceiveRealData(object sender, Datum e)
        {
            if (api.OnReceiveBalance)
            {
                if (checkBox.CheckState.Equals(CheckState.Checked) && (api.Quantity > 0 && e.Price > api.PurchasePrice + RevenueTick || api.Quantity < 0 && e.Price < api.PurchasePrice - RevenueTick))
                {
                    RevenueTick += 0.05;
                    api.OnReceiveBalance = false;
                    api.OnReceiveOrder(new MarketOrder
                    {
                        SlbyTP = api.Quantity > 0 ? "1" : "2"
                    });
                    button.Text = RevenueTick.ToString();
                    button.ForeColor = Color.Maroon;
                    return;
                }
                if (api.Quantity > 0 && api.PurchasePrice - StopLossTick > e.Price || api.Quantity < 0 && api.PurchasePrice + StopLossTick < e.Price)
                {
                    api.OnReceiveBalance = false;
                    StopLossTick *= 2;
                    api.OnReceiveOrder(new MarketOrder
                    {
                        Qty = Math.Abs(api.Quantity),
                        SlbyTP = api.Quantity > 0 ? "1" : "2"
                    });
                    checkBox.ForeColor = Color.DeepSkyBlue;
                    checkBox.Text = StopLossTick.ToString();
                    button.ForeColor = Color.DeepSkyBlue;
                }
                return;
            }
        }
        private void CheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            checkBox.Text = checkBox.CheckState.Equals(CheckState.Checked) ? "UseAll" : "OnlyStopLoss";
        }
        private void Button_Click(object sender, EventArgs e)
        {
            RevenueTick = 0.15;
            button.ForeColor = Color.Gold;
            button.Text = RevenueTick.ToString();
        }
        private void GetChart()
        {
            try
            {
                foreach (string rd in new Fetch())
                {
                    string[] arr = rd.Split(',');
                    api.Retention = arr[0];
                }
            }
            catch (Exception ex)
            {
                Box.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", 3750);
                Environment.Exit(0);
            }
        }
        private void OnReceiveExit(object sender, ForceQuit e)
        {
            SendQuit?.Invoke(this, new ForceQuit(1));
        }
        private readonly PublicFutures api;
        public event EventHandler<ForceQuit> SendQuit;
    }
}