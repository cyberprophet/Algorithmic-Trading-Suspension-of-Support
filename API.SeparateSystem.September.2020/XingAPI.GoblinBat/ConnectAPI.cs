using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShareInvest.Analysis;
using ShareInvest.Analysis.XingAPI;
using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.Controls;
using ShareInvest.EventHandler;
using ShareInvest.FindByName;
using ShareInvest.Interface;
using ShareInvest.Interface.XingAPI;
using ShareInvest.XingAPI.Catalog;

namespace ShareInvest.XingAPI
{
    public sealed partial class ConnectAPI : UserControl, ISecuritiesAPI<SendSecuritiesAPI>
    {
        void ButtonStartProgressClick(object sender, EventArgs e)
        {
            Start = true;

            if (textCertificate.Text.Length > 9 && textIdentity.Text.Length < 9 && textPassword.Text.Length < 9)
                BeginInvoke(new Action(() =>
                {
                    API = Connect.GetInstance(new Privacies
                    {
                        Identity = textIdentity.Text,
                        Password = textPassword.Text,
                        Certificate = textCertificate.Text
                    },
                    new LoadServer
                    {
                        Server = checkDemo.Checked ? demo : hts,
                        Date = labelMessage.Text
                    });
                    if (API is Connect api)
                        Send?.Invoke(this, new SendSecuritiesAPI(FormWindowState.Minimized, string.IsNullOrEmpty(privacy.SecurityAPI) ? new Accounts(api.Accounts) : new Accounts(securites[1], securites[4])));
                }));
            else
                buttonStartProgress.Text = error;
        }
        void OnReceiveControls(object sender, EventArgs e)
        {
            if (sender is TextBox text && text.ForeColor.Equals(Color.DarkGray))
            {
                if (e is PreviewKeyDownEventArgs key && key.KeyData.Equals(Keys.Tab))
                    return;

                text.UseSystemPasswordChar = text.Name.Equals(identity) == false;
                text.ForeColor = Color.Black;

                if (e is MouseEventArgs || text.UseSystemPasswordChar)
                    text.Text = string.Empty;

                text.MouseDown -= OnReceiveControls;
                text.PreviewKeyDown -= OnReceiveControls;
            }
        }
        void FindControlRecursive(Control control)
        {
            foreach (Control sub in control.Controls)
                if (sub.Controls.Count == 0 && sub.GetType().Name.Equals(text))
                {
                    var con = sub.Name.FindByName<TextBox>(this);
                    con.MouseDown += OnReceiveControls;
                    con.PreviewKeyDown += OnReceiveControls;
                }
                else if (sub.Controls.Count > 0)
                    FindControlRecursive(sub);
        }
        bool DistinguishBetweenDayAndNight(DateTime now) => now.Hour < 0x10 && now.Hour > 5;
        public IAccountInformation SetPrivacy(IAccountInformation privacy)
        {
            var ai = new AccountInformation
            {
                Identity = textIdentity.Text,
                Account = privacy.AccountNumber,
                Server = checkDemo.Checked
            };
            if (API is Connect api)
            {
                var name = api.SetAccountName(privacy.AccountNumber, privacy.AccountPassword);
                Invoke(new Action(async () =>
                {
                    ai.Name = name.Item2;
                    ai.Nick = name.Item1;

                    if (checkPrivacy.Checked && int.MaxValue > await new Secrecy().Encrypt(this.privacy, textIdentity.Text, textPassword.Text, textCertificate.Text, privacy.AccountNumber, privacy.AccountPassword, checkDemo.Checked))
                        Console.WriteLine(ai.Nick);
                }));
            }
            return ai;
        }
        public void StartProgress(Codes codes)
        {
            if (codes.Code.Length == 8 && Codes.Add(codes) && API is Connect api)
                api.Request.RequestTrData(new Task(() => (codes.Code[1].Equals('0') ? new T2101() as IQuerys : new T8402())?.QueryExcute(codes.Code)));
        }
        public void StartProgress() => buttonStartProgress.PerformClick();
        public void SetForeColor(Color color, string remain)
        {
            labelXingAPI.ForeColor = color;
            labelMessage.Text = remain;
        }
        public int SetStrategics(IStrategics strategics)
        {
            switch (strategics)
            {
                case TrendFollowingBasicFutures tf:
                    var index = strategics.Code.Length == 8 && strategics.Code[1].Equals('0');
                    Connect.HoldingStock[strategics.Code] = new HoldingStocks(tf)
                    {
                        Code = strategics.Code,
                        Current = index ? 0D : 0,
                        Purchase = index ? 0D : 0,
                        Quantity = 0,
                        Rate = 0,
                        Revenue = 0,
                        Offer = index ? 0D : 0,
                        Bid = index ? 0D : 0
                    };
                    break;

                case TrendsInStockPrices ts:
                    Connect.HoldingStock[strategics.Code] = new HoldingStocks(ts)
                    {
                        Code = strategics.Code,
                        Current = 0,
                        Purchase = 0,
                        Quantity = 0,
                        Rate = 0,
                        Revenue = 0
                    };
                    break;
            }
            return Connect.HoldingStock.Count;
        }
        public Collect SetToCollect(string code)
        {
            var access = new Secrecy().GetGrantAccess(privacy.Security);

            if (string.IsNullOrEmpty(access) == false)
            {
                if (Connect.Collection == null)
                {
                    Connect.Collection = new Dictionary<string, Collect>();
                    Access = access;
                }
                if (code.Length == 8 && Connect.Collection.ContainsKey(code) == false)
                {
                    var collect = new Collect(code);
                    Connect.Collection[code] = collect;

                    return collect;
                }
            }
            return null;
        }
        public void SendTransmitCommand()
        {
            if (Connect.Collection != null)
                foreach (var kv in Connect.Collection)
                    if (kv.Value.Count > 0)
                        kv.Value.SendTransmitCommand(kv.Key);
        }
        public ICharts<SendSecuritiesAPI> Stocks
        {
            get;
        }
        public ICharts<SendSecuritiesAPI> Options
        {
            get;
        }
        public IEnumerable<Holding> HoldingStocks
        {
            get
            {
                foreach (var ctor in Connect.HoldingStock)
                    yield return ctor.Value ?? null;
            }
        }
        public IQuerys<SendSecuritiesAPI> JIF
        {
            get;
        }
        public IQuerys<SendSecuritiesAPI>[] Maturity => new IQuerys<SendSecuritiesAPI>[]
        {
            new Maturity.T8401(),
            new Maturity.T8432(),
            new Maturity.T8435()
        };
        public IQuerys<SendSecuritiesAPI>[] ConvertTheCodeToName => DistinguishBetweenDayAndNight(DateTime.Now) ? new IQuerys<SendSecuritiesAPI>[]
        {
            new T8430(),
            new T8435(),
            new T9943(),
            new T8401(),
            new T8432(),
            new T8433(),
            new MMDAQ91200()
        } : new IQuerys<SendSecuritiesAPI>[]
        {
            new T8437()
        };
        public IOrders<SendSecuritiesAPI>[] Orders => DistinguishBetweenDayAndNight(DateTime.Now) ? new IOrders<SendSecuritiesAPI>[]
        {
            new CFOAT00100(),
            new CFOAT00200(),
            new CFOAT00300()
        } : new IOrders<SendSecuritiesAPI>[]
        {
            new CEXAT11100(),
            new CEXAT11200(),
            new CEXAT11300(),
            new CCEAT00100(),
            new CCEAT00200(),
            new CCEAT00300()
        };
        public IQuerys<SendSecuritiesAPI>[] Querys => DistinguishBetweenDayAndNight(DateTime.Now) ? new IQuerys<SendSecuritiesAPI>[]
        {
            new CFOBQ10500(),
            new T0441()
        } : new IQuerys<SendSecuritiesAPI>[]
        {
            new CCEAQ50600(),
            new CEXAQ31200()
        };
        public IReals[] Conclusion => DistinguishBetweenDayAndNight(DateTime.Now) ? new IReals[]
        {
            new C01(),
            new H01(),
            new O01()
        } : new IReals[]
        {
            new EU0(),
            new EU1(),
            new EU2(),
            new CM0(),
            new CM1(),
            new CM2()
        };
        public IReals[] Reals => DistinguishBetweenDayAndNight(DateTime.Now) ? new IReals[]
        {
            new FC0(),
            new FH0()
        } : new IReals[]
        {
            new EC0(),
            new EH0(),
            new NC0(),
            new NH0()
        };
        public dynamic API
        {
            get; private set;
        }
        public bool Start
        {
            get; private set;
        }
        public ConnectAPI(Privacies privacy)
        {
            int index = 0;
            this.privacy = privacy;
            InitializeComponent();

            foreach (Control control in Controls)
                FindControlRecursive(control);

            if (string.IsNullOrEmpty(privacy.SecurityAPI) == false)
            {
                securites = new Secrecy().Decipher(privacy.Security, privacy.SecurityAPI);
                textCertificate.Text = securites[index++];
                textPassword.Text = securites[++index];
                textIdentity.Text = securites[++index];
                checkPrivacy.CheckState = CheckState.Checked;
                textPassword.UseSystemPasswordChar = true;
                textCertificate.UseSystemPasswordChar = true;
            }
            Codes = new HashSet<Codes>();
            Strategics = new HashSet<IStrategics>();
            Stocks = new T8411();
            Options = new T8414();
            JIF = new JIF();
        }
        public HashSet<IStrategics> Strategics
        {
            get; private set;
        }
        public static HashSet<Codes> Codes
        {
            get; private set;
        }
        public string Access
        {
            get; private set;
        }
        readonly string[] securites;
        readonly Privacies privacy;
        public event EventHandler<SendSecuritiesAPI> Send;
    }
}