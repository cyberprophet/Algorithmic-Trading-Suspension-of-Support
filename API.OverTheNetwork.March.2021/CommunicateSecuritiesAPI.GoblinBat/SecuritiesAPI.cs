using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
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
        internal SecuritiesAPI(dynamic param, ISecuritiesAPI<SendSecuritiesAPI> connect)
        {
            InitializeComponent();
            this.connect = connect;
            timer.Start();
            client = GoblinBat.GetInstance();
            random = new Random(Guid.NewGuid().GetHashCode());
            Codes = new Queue<string>();
            GetTheCorrectAnswer = new int[Security.Initialize(param)];
            miss = new Stack<Catalog.Models.Codes>();
        }
        void OnReceiveSecuritiesAPI(object sender, SendSecuritiesAPI e) => BeginInvoke(new Action(async () =>
        {
            switch (e.Convey)
            {
                case Tuple<string, string, string, string, int> tr:
                    await RegisterStocksItems(new Catalog.Models.Codes
                    {
                        Code = tr.Item1,
                        Name = tr.Item2,
                        MaturityMarketCap = tr.Item3,
                        Price = tr.Item4,
                        MarginRate = tr.Item5
                    }, sender);
                    return;

                case Tuple<string, string> request:
                    if (request.Item2.Length != 8)
                        Codes.Enqueue(string.Concat(request.Item1, '_', request.Item2));

                    (sender as OpenAPI.ConnectAPI).InputValueRqData(string.Concat(instance, request.Item1), request.Item2).Send += OnReceiveSecuritiesAPI;
                    return;

                case Catalog.Models.Codes codes:
                    if (string.IsNullOrEmpty(await client.PutContextAsync(codes) as string))
                        Base.SendMessage(sender.GetType(), codes.Name, codes.MaturityMarketCap);

                    return;

                case string message:
                    notifyIcon.Text = message;
                    return;

                case string[] accounts:
                    foreach (var str in accounts)
                        if (str.Length == 10 && str[^2..].CompareTo("32") < 0)
                            connect.Writer.WriteLine(str);

                    worker.RunWorkerAsync(connect.ConnectToReceiveRealTime);
                    return;

                case short error:
                    Dispose((Control)connect);
                    return;
            }
        }));
        async Task RegisterStocksItems(Catalog.Models.Codes register, object sender)
        {
            if (await client.PutContextAsync(register) is string name && register.Name.Equals(name))
            {
                if (register.Code.Length == 8)
                {
                    if (Codes.TryPeek(out string param) && param.Length > 8)
                    {
                        var temp = Codes.Dequeue().Split('_');
                        (connect as OpenAPI.ConnectAPI).RemoveValueRqData(temp[0], temp[^1]).Send -= OnReceiveSecuritiesAPI;
                    }
                    (connect as OpenAPI.ConnectAPI).RemoveValueRqData(sender.GetType().Name, register.Code).Send -= OnReceiveSecuritiesAPI;
                }
                if (register.Code.Length == 6 || register.Code.Length == 8 && register.Code[1] == '0')
                    Codes.Enqueue(register.Code);
            }
            else
            {
                Base.SendMessage(sender.GetType(), register.Name);
                miss.Push(register);
            }
        }
        void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is NamedPipeServerStream server)
                using (var sr = new StreamReader(server))
                    try
                    {
                        while (server.IsConnected)
                        {
                            var param = sr.ReadLine();

                            if (string.IsNullOrEmpty(param) == false)
                            {
                                Base.SendMessage(sender.GetType(), param);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Base.SendMessage(sender.GetType(), ex.StackTrace);
                    }
                    finally
                    {
                        Dispose((Control)connect);
                    }
        }
        void TimerTick(object sender, EventArgs e)
        {
            if (connect == null)
            {
                timer.Stop();
                strip.ItemClicked -= StripItemClicked;
                Dispose((Control)connect);
            }
            else if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) == false)
            {
                for (int i = 0; i < GetTheCorrectAnswer.Length; i++)
                {
                    var temp = 1 + random.Next(0, 0x4B0) * (i + 1);
                    GetTheCorrectAnswer[i] = temp < 0x4B1 ? temp : 0x4B0 - i;
                }
                WindowState = FormWindowState.Minimized;
            }
            else if (connect.Start)
            {
                if (miss.Count > 0 && Codes.Count == Miss)
                    BeginInvoke(new Action(async () => await RegisterStocksItems(miss.Pop(), this)));

                else
                    Miss = Codes.Count;
            }
            else if (Visible == false && ShowIcon == false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized))
            {
                var now = DateTime.Now;
                now = now.DayOfWeek switch
                {
                    DayOfWeek.Sunday => now.AddDays(1),
                    DayOfWeek.Saturday => now.AddDays(2),
                    DayOfWeek weeks when weeks.Equals(DayOfWeek.Friday) && now.Hour > 8 => now.AddDays(3),
                    _ => now.Hour > 8 || Array.Exists(Base.Holidays, o => o.Equals(now.ToString("yyMMdd"))) ? now.AddDays(1) : now,
                };
                var remain = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0) - DateTime.Now;
                notifyIcon.Text = Base.GetRemainingTime(remain);

                if (connect.Start == false && remain.TotalMinutes < 0x1F && now.Hour == 8 && now.Minute > 0x1E &&
                    (remain.TotalMinutes < 0x15 || Array.Exists(GetTheCorrectAnswer, o => o == random.Next(0, 0x4B2))))
                    StartProgress((Control)connect);
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
            get;
        }
        int[] GetTheCorrectAnswer
        {
            get;
        }
        int Miss
        {
            get; set;
        }
        const string instance = "ShareInvest.OpenAPI.Catalog.";
        readonly Random random;
        readonly GoblinBat client;
        readonly Stack<Catalog.Models.Codes> miss;
        readonly ISecuritiesAPI<SendSecuritiesAPI> connect;
    }
}