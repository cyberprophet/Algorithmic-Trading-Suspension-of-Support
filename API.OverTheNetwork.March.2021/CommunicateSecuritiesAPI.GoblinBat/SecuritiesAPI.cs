using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ShareInvest.Client;
using ShareInvest.EventHandler;
using ShareInvest.Interface;

namespace ShareInvest
{
    sealed partial class SecuritiesAPI : Form
    {
        internal bool Repeat
        {
            get; private set;
        }
        internal SecuritiesAPI(ISecuritiesAPI<SendSecuritiesAPI> connect)
        {
            InitializeComponent();
            this.connect = connect;
            timer.Start();
            client = GoblinBat.GetInstance();
            Codes = new Queue<string>();
        }
        void OnReceiveSecuritiesAPI(object sender, SendSecuritiesAPI e) => BeginInvoke(new Action(async () =>
        {
            switch (e.Convey)
            {
                case Tuple<string, string, string, string> conclusion:
                    var conclusion_order = await client.PostContextAsync(sender.GetType(), new Catalog.Models.Conclusion
                    {
                        Code = conclusion.Item1,
                        Time = conclusion.Item2,
                        Price = conclusion.Item3,
                        Volume = conclusion.Item4
                    });
                    if (conclusion_order is not null)
                    {

                    }
                    return;

                case Tuple<string, StringBuilder> quotes:
                    var quotes_order = await client.PostContextAsync(sender.GetType(), new Catalog.Models.Quotes
                    {
                        Code = quotes.Item1,
                        Datum = quotes.Item2.ToString()
                    });
                    if (quotes_order is not null)
                    {

                    }
                    return;

                case Tuple<string, double, double, double> futures:
                    await client.PutContextAsync(new Catalog.Models.Priority
                    {
                        Code = futures.Item1,
                        Current = futures.Item2,
                        Offer = futures.Item3,
                        Bid = futures.Item4
                    });
                    return;

                case Tuple<string, int, int, int> stocks:
                    await client.PutContextAsync(new Catalog.Models.Priority
                    {
                        Code = stocks.Item1,
                        Current = stocks.Item2,
                        Offer = stocks.Item3,
                        Bid = stocks.Item4
                    });
                    return;

                case Tuple<string, int, int> bs:
                    await client.PutContextAsync(new Catalog.Models.Priority
                    {
                        Code = bs.Item1,
                        Current = int.MinValue,
                        Offer = bs.Item2,
                        Bid = bs.Item3
                    });
                    return;

                case Tuple<string, string[]> account:
                    foreach (var ctor in (sender as OpenAPI.ConnectAPI)?.ConnectToReceiveRealTime)
                        ctor.Send += OnReceiveSecuritiesAPI;

                    return;

                case Tuple<string, string> request:
                    if (request.Item2.Length != 8)
                        Codes.Enqueue(string.Concat(request.Item1, '_', request.Item2));

                    (sender as OpenAPI.ConnectAPI).InputValueRqData(string.Concat(instance, request.Item1), request.Item2).Send += OnReceiveSecuritiesAPI;
                    return;

                case Tuple<string, string, string> operation:

                    return;

                case Tuple<string, string, string, string, int> tr:
                    await client.PutContextAsync(new Catalog.Models.Codes
                    {
                        Code = tr.Item1,
                        Name = tr.Item2,
                        MaturityMarketCap = tr.Item3,
                        Price = tr.Item4,
                        MarginRate = tr.Item5
                    });
                    if (tr.Item1.Length == 8)
                    {
                        if (Codes.TryPeek(out string param) && param.Length > 8)
                        {
                            var temp = Codes.Dequeue().Split('_');
                            (connect as OpenAPI.ConnectAPI).RemoveValueRqData(temp[0], temp[^1]).Send -= OnReceiveSecuritiesAPI;
                        }
                        (connect as OpenAPI.ConnectAPI).RemoveValueRqData(sender.GetType().Name, tr.Item1).Send -= OnReceiveSecuritiesAPI;
                    }
                    if (tr.Item1.Length == 6 || tr.Item1.Length == 8 && tr.Item1[1] == '0')
                        Codes.Enqueue(tr.Item1);

                    return;

                case Catalog.Models.Codes codes:
                    await client.PutContextAsync(codes);
                    return;

                case string message:
                    notifyIcon.Text = message;
                    return;

                case short error:
                    Repeat = error == -0x6A;
                    Dispose((Control)connect);
                    return;
            }
        }));
        void TimerTick(object sender, EventArgs e)
        {
            if (connect == null)
            {
                timer.Stop();
                Dispose((Control)connect);
            }
            else if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) == false)
            {
                WindowState = FormWindowState.Minimized;
            }
            else if (Visible == false && ShowIcon == false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized))
            {

            }
        }
        void SecuritiesResize(object sender, EventArgs e) => BeginInvoke(new Action(() =>
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                SuspendLayout();
                Visible = false;
                ShowIcon = false;
                notifyIcon.Visible = true;
                ResumeLayout();
            }
        }));
        void StripItemClicked(object sender, ToolStripItemClickedEventArgs e) => BeginInvoke(new Action(() =>
        {
            if (e.ClickedItem.Name.Equals(reference.Name))
            {
                if (e.ClickedItem.Text.Equals("연결"))
                {
                    e.ClickedItem.Text = "조회";
                    StartProgress((Control)connect);
                }
                else
                {

                }
            }
            else
                Close();
        }));
        void StartProgress(Control connect)
        {
            Controls.Add(connect);
            connect.Dock = DockStyle.Fill;
            connect.Show();
            FormBorderStyle = FormBorderStyle.None;
            CenterToScreen();
            this.connect.Send += OnReceiveSecuritiesAPI;
            this.connect.StartProgress();
        }
        void Dispose(Control connect)
        {
            connect.Dispose();
            Dispose();
        }
        Queue<string> Codes
        {
            get; set;
        }
        const string instance = "ShareInvest.OpenAPI.Catalog.";
        readonly GoblinBat client;
        readonly ISecuritiesAPI<SendSecuritiesAPI> connect;
    }
}