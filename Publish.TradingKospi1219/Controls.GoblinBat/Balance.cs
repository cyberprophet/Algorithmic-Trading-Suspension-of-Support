using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Const;
using ShareInvest.EventHandler;
using ShareInvest.Interface;
using ShareInvest.Log.Message;
using ShareInvest.OpenAPI;

namespace ShareInvest.Controls
{
    public partial class Balance : UserControl
    {
        public static Balance Get()
        {
            if (bal == null)
                bal = new Balance();

            return bal;
        }
        public void OnReceiveLiquidate(object sender, Liquidate e)
        {
            EnCash++;
            OrderType = e.EnCash.SlbyTP.Equals("1") ? "201" : "301";
        }
        private void OnReceiveBalance(object sender, Holding e)
        {
            if (EnCash > 0)
            {
                EnCash--;
                int price = 0;
                string code = string.Empty;

                foreach (string en in e.Hold)
                {
                    if (en.Equals(string.Empty))
                        break;

                    string[] temp = en.Split(';');

                    if (temp[0].Length > 0 && (temp[0].Substring(0, 3).Equals("101") || temp[0].Substring(0, 3).Equals(OrderType)))
                        continue;

                    int close = int.Parse(temp[5]);

                    if (close > price)
                    {
                        code = temp[0];
                        price = close;
                    }
                }
                if (price > 0)
                    api.OnReceiveOrder(new PurchaseInformation
                    {
                        Code = code,
                        SlbyTP = "1",
                        OrdTp = ((int)IStrategy.OrderType.시장가).ToString(),
                        Price = string.Empty,
                        Qty = 1
                    });
                BeginInvoke(new Action(() => new LogMessage().Record("Options", string.Concat(DateTime.Now.ToLongTimeString(), "*", code, "*", (price / (double)100).ToString("N2"), "*", "Sell"))));
            }
            balGrid.SuspendLayout();
            balGrid.Rows.Clear();
            balGrid.AutoSize = true;

            foreach (string info in e.Hold)
            {
                string[] arr = new string[7];
                int i = 0;

                foreach (string val in info.Split(';'))
                {
                    if (val.Equals(string.Empty))
                        break;

                    switch (i)
                    {
                        case 0:
                        case 1:
                            arr[i++] = val;
                            break;

                        case 2:
                            arr[i++] = val.Equals("1") ? "매도" : "매수";
                            break;

                        case 3:
                        case 6:
                            arr[i++] = int.Parse(val).ToString("N0");
                            break;

                        case 4:
                        case 5:
                            arr[i++] = (double.Parse(val) / 100).ToString("N2");
                            break;
                    }
                }
                if (arr[0] != null)
                    balGrid.Rows.Add(arr);
            }
            if (balGrid.Rows.Count > 0)
            {
                balGrid.Show();
                balGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                balGrid.Cursor = Cursors.Hand;
                balGrid.AutoResizeRows();
                balGrid.AutoResizeColumns();
                balGrid.ResumeLayout();
                SendReSize?.Invoke(this, new GridReSize(balGrid.Rows.GetRowsHeight(DataGridViewElementStates.None), balGrid.Rows.Count));

                return;
            }
            balGrid.Hide();
        }
        private string OrderType
        {
            get; set;
        }
        private int EnCash
        {
            get; set;
        }
        private Balance()
        {
            InitializeComponent();
            balGrid.ColumnCount = 7;
            balGrid.BackgroundColor = Color.FromArgb(121, 133, 130);
            api = ConnectAPI.Get();
            api.SendHolding += OnReceiveBalance;

            for (int i = 0; i < columns.Length; i++)
                balGrid.Columns[i].Name = columns[i];
        }
        private readonly string[] columns = { "종목코드", "종목명", "구분", "수량", "매입가", "현재가", "평가손익" };
        private readonly ConnectAPI api;
        private static Balance bal;
        public event EventHandler<GridReSize> SendReSize;
    }
}