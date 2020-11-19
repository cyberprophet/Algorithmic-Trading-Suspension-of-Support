using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShareInvest.Catalog.Models;
using ShareInvest.Client;
using ShareInvest.EventHandler;
using ShareInvest.Interface;

namespace ShareInvest
{
    sealed partial class SecuritiesAPI : Form
    {
        [Conditional("DEBUG")]
        static void PreventsFromRunningAgain(FormClosingEventArgs e) => e.Cancel = true;
        internal SecuritiesAPI(dynamic param, ISecuritiesAPI<SendSecuritiesAPI> connect)
        {
            InitializeComponent();
            this.connect = connect;
            timer.Start();
            client = GoblinBat.GetInstance();
            random = new Random(Guid.NewGuid().GetHashCode());
            Codes = new Queue<string>();
            var normalize = Security.Initialize(param);
            GetTheCorrectAnswer = new int[normalize.Item1];
            miss = new Stack<Codes>();
            collection = new Dictionary<string, Codes>();
            this.normalize = normalize.Item2;
        }
        void OnReceiveInformationTheDay() => BeginInvoke(new Action(async () =>
        {
            if (Codes.Count > 0)
            {
                if (await client.PostContextAsync(new Retention { Code = Codes.Dequeue() }) is Retention retention)
                    switch (connect)
                    {
                        case OpenAPI.ConnectAPI o when string.IsNullOrEmpty(retention.Code) == false:
                            o.InputValueRqData(string.Concat(instance, retention.Code.Length == 8 ? (retention.Code[0] > '1' ? "Opt50066" : "Opt50028") : "Opt10079"), string.Concat(retention.Code, ';', retention.LastDate)).Send += OnReceiveSecuritiesAPI;
                            return;
                    }
                OnReceiveInformationTheDay();
            }
            else
                Dispose(0x1CE3);
        }));
        void OnReceiveSecuritiesAPI(object sender, SendSecuritiesAPI e) => BeginInvoke(new Action(async () =>
        {
            switch (e.Convey)
            {
                case Tuple<string, string, string, string, int> tr:
                    var param = new Codes
                    {
                        Code = tr.Item1,
                        Name = tr.Item2,
                        MaturityMarketCap = tr.Item3,
                        Price = tr.Item4,
                        MarginRate = tr.Item5
                    };
                    await RegisterStocksItems(param, sender);
                    collection[tr.Item1] = param;
                    return;

                case Tuple<string, string> request:
                    if (request.Item2.Length != 8)
                        Codes.Enqueue(string.Concat(request.Item1, '_', request.Item2));

                    (sender as OpenAPI.ConnectAPI).InputValueRqData(string.Concat(instance, request.Item1), request.Item2).Send += OnReceiveSecuritiesAPI;
                    return;

                case Codes codes:
                    if (string.IsNullOrEmpty(await this.client.PutContextAsync(codes) as string))
                        Base.SendMessage(sender.GetType(), codes.Name, codes.MaturityMarketCap);

                    return;

                case string message:
                    notifyIcon.Text = string.Concat(DateTime.Now.ToLongTimeString(), " ", message);
                    return;

                case Queue<string[]> hold:
                    while (hold.Count > 0)
                    {
                        var ing = hold.Dequeue();
                        var convey = string.Empty;

                        if (ing[0].Length == 8 && int.TryParse(ing[4], out int quantity) && double.TryParse(ing[9], out double fRate) && long.TryParse(ing[8], out long fValuation) && double.TryParse(ing[6], out double fCurrent) && double.TryParse(ing[5], out double fPurchase))
                            convey = string.Concat(inquiry, ing[0], ';', ing[1].Equals(ing[0]) && collection.TryGetValue(ing[1], out Codes c) ? c.Name : ing[1], ';', ing[2].Equals("1") ? -quantity : quantity, ';', fPurchase, ';', fCurrent, ';', fValuation, ';', fRate * 1e-2);

                        else if (ing[3].Length > 0 && ing[3][0] == 'A' && double.TryParse(ing[0xC]?.Insert(6, "."), out double ratio) && long.TryParse(ing[0xB], out long valuation) && int.TryParse(ing[6], out int reserve) && uint.TryParse(ing[8], out uint purchase) && uint.TryParse(ing[7], out uint current))
                            convey = string.Concat(inquiry, ing[3][1..].Trim(), ';', ing[4].Trim(), ';', reserve, ';', purchase, ';', current, ';', valuation, ';', ratio);

                        connect.Writer.WriteLine(convey);
                    }
                    (connect as OpenAPI.ConnectAPI).RemoveValueRqData(sender.GetType().Name, string.Concat(connect.Account, password)).Send -= OnReceiveSecuritiesAPI;
                    return;

                case Tuple<long, long> balance:
                    connect.Writer.WriteLine(string.Concat(inquiry, balance.Item1, ';', balance.Item2));
                    return;

                case Tuple<string, Stack<string>> charts:
                    switch (sender)
                    {
                        case OpenAPI.ConnectAPI o:
                            o.RemoveValueRqData(sender.GetType().Name, charts.Item1).Send -= OnReceiveSecuritiesAPI;
                            break;
                    }
                    if ((charts.Item1.Length == 8 ? (charts.Item1[0] > '1' ? await this.client.PostContextAsync(Catalog.Models.Convert.ToStoreInOptions(charts.Item1, charts.Item2)) : await this.client.PostContextAsync(Catalog.Models.Convert.ToStoreInFutures(charts.Item1, charts.Item2))) : await this.client.PostContextAsync(Catalog.Models.Convert.ToStoreInStocks(charts.Item1, charts.Item2))) > 0xC7)
                        OnReceiveInformationTheDay();

                    break;

                case string[] accounts:
                    foreach (var str in accounts)
                        if (str.Length == 10 && str[^2..].CompareTo("32") < 0)
                            connect.Writer.WriteLine(str);

                    var client = new NamedPipeClientStream(".", normalize, PipeDirection.In, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation);
                    await client.ConnectAsync();
                    worker.RunWorkerAsync(client);
                    return;

                case short error:
                    Dispose(connect as Control);
                    return;
            }
        }));
        async Task RegisterStocksItems(Codes register, object sender)
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
        void RequestBalanceInquiry()
        {
            if (string.IsNullOrEmpty(connect.Account) == false)
                if (connect is OpenAPI.ConnectAPI o)
                {
                    if (connect.Account[^2..].Equals("31"))
                    {

                    }
                    else
                        o.InputValueRqData(string.Concat(instance, "Opw00005"), string.Concat(connect.Account, password)).Send += OnReceiveSecuritiesAPI;
                }
        }
        void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is NamedPipeClientStream client)
                using (var sr = new StreamReader(client))
                    try
                    {
                        while (client.IsConnected)
                        {
                            var param = sr.ReadLine();

                            if (string.IsNullOrEmpty(param) == false)
                            {
                                var temp = param.Split('|');

                                if (temp[0].Equals("Operation"))
                                    switch (temp[^1])
                                    {
                                        case "085500":
                                            miss.Clear();
                                            RequestBalanceInquiry();
                                            break;

                                        case "152000":
                                            break;

                                        case "8":
                                            OnReceiveInformationTheDay();
                                            break;

                                        case "d":
                                            Dispose(connect as Control);
                                            break;
                                    }
                                else if (temp[0].Equals("Security"))
                                {
                                    if (string.IsNullOrEmpty(temp[^1]))
                                    {

                                    }
                                    else
                                        connect.Account = temp[^1];
                                }
                                Base.SendMessage(sender.GetType(), temp[0], temp[^1]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Base.SendMessage(sender.GetType(), ex.StackTrace);
                    }
                    finally
                    {
                        client.Close();
                        client.Dispose();
                    }
        }
        void TimerTick(object sender, EventArgs e)
        {
            if (connect == null)
            {
                timer.Stop();
                strip.ItemClicked -= StripItemClicked;
                Dispose(connect as Control);
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

                if (notifyIcon.Text.Length == 0 || notifyIcon.Text.Length > 0xF && notifyIcon.Text[^5..].Equals(". . ."))
                {
                    notifyIcon.Text = connect.SecuritiesName;
                    timer.Interval = 0x3E15;
                }
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
                    StartProgress(connect as Control);
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
        void StripItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name.Equals(reference.Name))
            {
                if (e.ClickedItem.Text.Equals("연결"))
                {
                    e.ClickedItem.Text = "조회";
                    StartProgress(connect as Control);
                }
                else
                    switch (MessageBox.Show(look_up, connect.SecuritiesName, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
                    {
                        case DialogResult.Abort:

                            break;

                        case DialogResult.Retry:
                            RequestBalanceInquiry();
                            break;

                        case DialogResult.Ignore:
                            RequestTheMissingInformation();
                            break;
                    }
            }
            else
                Close();
        }
        void JustBeforeFormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseReason.UserClosing.Equals(e.CloseReason))
                switch (MessageBox.Show(form_exit, connect.SecuritiesName, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
                {
                    case DialogResult.OK:
                        PreventsFromRunningAgain(e);

                        if (e.Cancel == false)
                        {
                            if (connect.Writer != null)
                            {
                                Dispose(0xF75);

                                return;
                            }
                            Process.Start("shutdown.exe", "-r");

                            foreach (var process in Process.GetProcesses())
                                if (Security.API.Equals(process.ProcessName))
                                    process.Kill();
                        }
                        break;

                    case DialogResult.Cancel:
                        e.Cancel = e.CloseReason.Equals(CloseReason.UserClosing);
                        return;
                }
            Dispose(connect as Control);
        }
        void StartProgress(Control connect)
        {
            if (connect is Control)
            {
                Controls.Add(connect);
                connect.Dock = DockStyle.Fill;
                connect.Show();
                FormBorderStyle = FormBorderStyle.None;
                CenterToScreen();
                this.connect.Send += OnReceiveSecuritiesAPI;
                this.connect.StartProgress();
            }
            else
                Close();
        }
        void Dispose(Control connect)
        {
            if (connect is Control)
                connect.Dispose();

            Dispose();
        }
        void Dispose(int milliseconds)
        {
            connect.Writer.WriteLine(string.Concat("장시작시간|", GetType(), "|d;", DateTime.Now.ToString("HH:mm:ss.ffff"), ';', typeof(Catalog.OpenAPI.Operation)));
            Thread.Sleep(milliseconds);
            Dispose(connect as Control);
        }
        [Conditional("DEBUG")]
        void RequestTheMissingInformation()
        {
            var message = string.Empty;

            switch (MessageBox.Show(GetType().AssemblyQualifiedName, Name, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3))
            {
                case DialogResult.Yes:
                    message = string.Concat("장시작시간|", GetType(), "|8;", DateTime.Now.ToString("HH:mm:ss.ffff"), ';', typeof(Catalog.OpenAPI.Operation));
                    break;

                case DialogResult.No:
                    message = string.Concat("장시작시간|", GetType(), "|0;085500;", DateTime.Now.ToString("HH:mm:ss.ffff"), ';', typeof(Catalog.OpenAPI.Operation));
                    break;

                case DialogResult.Cancel:
                    return;
            }
            if (string.IsNullOrEmpty(message) == false)
                connect.Writer.WriteLine(message);
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
        const string look_up = "'조회'는 장시작 5분전 자동으로 실행됩니다.\n\n강제로 프로그램을 실행하지 않았다면\n사용을 '중단'할 것을 권장합니다.\n\n(중단)\n'조회'를 취소합니다.\n\n(재시도)\n'조회'가 실행되면 장시작 설정으로 초기화됩니다.\n\n(무시)\n'관리자'만 실행가능합니다.";
        const string form_exit = "사용자 종료시 데이터 소실 및 오류를 초래합니다.\n\n그래도 종료하시겠습니까?\n\n프로그램 종료후 자동으로 재부팅됩니다.";
        const string instance = "ShareInvest.OpenAPI.Catalog.";
        const string password = ";;00";
        const string inquiry = "조회|";
        readonly string normalize;
        readonly Random random;
        readonly GoblinBat client;
        readonly Stack<Codes> miss;
        readonly Dictionary<string, Codes> collection;
        readonly ISecuritiesAPI<SendSecuritiesAPI> connect;
    }
}