using System.Drawing;
using System.Windows.Forms;
using ShareInvest.EventHandler;
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
        private void OnReceiveBalance(object sender, Holding e)
        {
            balGrid.Rows.Clear();
            balGrid.AutoSize = true;

            foreach (string info in e.Hold)
            {
                string[] arr = new string[7];
                int i = 0;

                if (info.Length > 0)
                {
                    foreach (string val in info.Split(';'))
                    {
                        string temp = val;

                        switch (i)
                        {
                            case 0:
                            case 1:
                                break;

                            case 2:
                                temp = val.Equals("1") ? "매도" : "매수";
                                break;

                            case 3:
                            case 6:
                                temp = int.Parse(temp).ToString("N0");
                                break;

                            case 4:
                            case 5:
                                temp = (double.Parse(temp) / 100).ToString("N2");
                                break;

                            case 7:
                            case 8:
                                break;
                        }
                        if (i > 6)
                            break;

                        arr[i++] = temp;
                    }
                    balGrid.Rows.Add(arr);
                }
            }
            if (balGrid.Rows.Count < 1)
            {
                balGrid.Hide();

                return;
            }
            balGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            balGrid.Cursor = Cursors.Hand;
        }
        private Balance()
        {
            InitializeComponent();
            balGrid.ColumnCount = 7;
            balGrid.BackgroundColor = Color.FromArgb(121, 133, 130);
            ConnectAPI.Get().SendHolding += OnReceiveBalance;

            for (int i = 0; i < columns.Length; i++)
                balGrid.Columns[i].Name = columns[i];
        }
        private readonly string[] columns = { "종목코드", "종목명", "구분", "수량", "매입가", "현재가", "평가손익" };
        private static Balance bal;
    }
}