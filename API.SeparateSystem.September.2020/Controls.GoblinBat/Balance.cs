using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.Controls
{
    public partial class Balance : UserControl
    {
        public Balance(AccountInformation info)
        {
            InitializeComponent();
            textIdentity.Text = info.Identity;
            textAccount.Text = (info.Account.Length == 11 ? info.Account : info.Account?.Insert(4, "-")).Insert(9, "-");
            textName.Text = info.Name;
            textServer.Text = info.Server.ToString();
            textServer.ForeColor = info.Server ? Color.Navy : Color.Maroon;
            data.ColumnCount = 8;
            data.BackgroundColor = Color.FromArgb(0x79, 0x85, 0x82);
            dIndex = new Dictionary<string, int>();

            foreach (Control control in panel.Controls)
                if (control is TextBox box)
                {
                    box.MouseLeave += OnResponseToMouse;
                    box.MouseUp += OnResponseToMouse;
                }
            for (int i = 0; i < columns.Length; i++)
                data.Columns[i].Name = columns[i];
        }
        public void OnReceiveDeposit(Tuple<long, long> param)
        {
            textAssets.Text = param.Item1.ToString("C0")?.Replace("-", string.Empty);
            textAvailable.Text = param.Item2.ToString("C0")?.Replace("-", string.Empty);
            textAssets.ForeColor = param.Item1 == 0 ? Color.Snow : param.Item1 > 0 ? Color.Maroon : Color.Navy;
            textAvailable.ForeColor = param.Item2 == 0 ? Color.Snow : param.Item2 > 0 ? Color.Maroon : Color.Navy;
        }
        public void OnReceiveDeposit(long available)
        {
            textAvailable.Text = available.ToString("C0")?.Replace("-", string.Empty);
            textAvailable.ForeColor = available == 0 ? Color.Snow : available > 0 ? Color.Maroon : Color.Navy;
        }
        public int OnReceiveBalance(Tuple<string, string, int, dynamic, dynamic, long, double> balance)
        {
            if (balance.Item3 == 0 && dIndex.TryGetValue(balance.Item1, out int rIndex) && dIndex.Remove(balance.Item1))
            {
                data.Rows.RemoveAt(rIndex);

                return data.Rows.GetRowsHeight(DataGridViewElementStates.None) + (data.Rows.Count == 0 ? 0 : 0x19);
            }
            var hasRows = dIndex.ContainsKey(balance.Item1);

            if (hasRows == false)
            {
                dIndex[balance.Item1] = data.Rows.Add(new string[] { balance.Item1, balance.Item2.Trim(), balance.Item3.ToString("N0"), balance.Item4.ToString("N0"), balance.Item5.ToString("N0"), balance.Item6.ToString("C0"), balance.Item7.ToString("P2"), "AXISCODE" });
                data.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                data.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                data.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                data.AutoResizeRows();
                data.AutoResizeColumns();
            }
            ChangeToCurrent(balance.Item1, hasRows, balance.Item5, balance.Item6, balance.Item7, balance.Item3, balance.Item4);

            return data.Rows.GetRowsHeight(DataGridViewElementStates.None) + (data.Rows.Count == 0 ? 0 : 0x19);
        }
        public void OnReceiveMessage(string message) => labelMessage.Text = string.Concat(DateTime.Now.ToLongTimeString(), " ", message);
        public void SetConnectHoldingStock(Holding stocks)
        {
            if (stocks != null)
                stocks.SendStocks += OnReceiveHoldingStocks;
        }
        public void SetDisconnectHoldingStock(Holding stocks)
        {
            if (stocks != null)
                stocks.SendStocks -= OnReceiveHoldingStocks;
        }
        void ChangeToCurrent(string code, bool hasRows, dynamic current, long revenue, double rate, int quantity, dynamic purchase)
        {
            if (dIndex.TryGetValue(code, out int index) && (hasRows && current.ToString("N0").Equals(data.Rows[index].Cells[4].Value.ToString())) == false)
            {
                if (revenue > 0)
                {
                    data.Rows[index].Cells[5].Style.ForeColor = Color.Maroon;
                    data.Rows[index].Cells[5].Style.SelectionForeColor = Color.FromArgb(0xB9062F);
                    data.Rows[index].Cells[6].Style.ForeColor = Color.Maroon;
                    data.Rows[index].Cells[6].Style.SelectionForeColor = Color.FromArgb(0xB9062F);
                }
                else if (revenue < 0)
                {
                    data.Rows[index].Cells[5].Style.ForeColor = Color.Navy;
                    data.Rows[index].Cells[5].Style.SelectionForeColor = Color.DeepSkyBlue;
                    data.Rows[index].Cells[6].Style.ForeColor = Color.Navy;
                    data.Rows[index].Cells[6].Style.SelectionForeColor = Color.DeepSkyBlue;
                }
                if (quantity > 0 && purchase is double)
                {
                    data.Rows[index].Cells[2].Style.ForeColor = Color.Maroon;
                    data.Rows[index].Cells[2].Style.SelectionForeColor = Color.FromArgb(0xB9062F);
                }
                else if (quantity < 0 && purchase is double)
                {
                    data.Rows[index].Cells[2].Style.ForeColor = Color.Navy;
                    data.Rows[index].Cells[2].Style.SelectionForeColor = Color.DeepSkyBlue;
                }
                data.Rows[index].Cells[5].Value = Math.Abs(revenue).ToString("C0");
                data.Rows[index].Cells[6].Value = Math.Abs(rate).ToString("P2");
                data.Rows[index].Cells[3].Value = purchase.ToString(purchase is double ? "N2" : "N0");
                data.Rows[index].Cells[2].Value = Math.Abs(quantity).ToString("N0");
                data.Rows[index].Cells[4].Value = current.ToString(current is double ? "N2" : "N0");
            }
            data.CurrentCell = data.Rows.Count > 1 && dIndex.ContainsKey(code) ? data.Rows[dIndex[code]].Cells[7] : null;
        }
        void OnReceiveHoldingStocks(object sender, SendHoldingStocks e) => BeginInvoke(new Action(() => ChangeToCurrent(e.Code, dIndex.ContainsKey(e.Code), e.Current, e.Revenue, e.Rate, e.Quantity, e.Purchase)));
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
        readonly Dictionary<string, int> dIndex;
    }
}