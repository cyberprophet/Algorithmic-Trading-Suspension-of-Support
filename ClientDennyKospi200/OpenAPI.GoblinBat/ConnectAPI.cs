using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AxKHOpenAPILib;
using ShareInvest.Catalog;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.Interface;

namespace ShareInvest.OpenAPI
{
    public class ConnectAPI : DistinctDate
    {
        public void SetAPI(AxKHOpenAPI axAPI)
        {
            this.axAPI = axAPI;
            axAPI.OnEventConnect += OnEventConnect;
            axAPI.OnReceiveTrData += OnReceiveTrData;
            axAPI.OnReceiveRealData += OnReceiveRealData;
            axAPI.OnReceiveChejanData += OnReceiveChejanData;
            axAPI.OnReceiveMsg += OnReceiveMsg;
        }
        public void StartProgress()
        {
            if (axAPI != null)
            {
                ErrorCode = axAPI.CommConnect();

                return;
            }
            Environment.Exit(0);
        }
        public void RemainingDay(string code)
        {
            request.RequestTrData(new Task(() => InputValueRqData(new Opt50001 { Value = code })));
        }
        public static ConnectAPI Get()
        {
            if (api == null)
                api = new ConnectAPI();

            return api;
        }
        private void InputValueRqData(ITR param)
        {
            string[] count = param.ID.Split(';'), value = param.Value.Split(';');
            int i, l = count.Length;

            for (i = 0; i < l; i++)
                axAPI.SetInputValue(count[i], value[i]);

            ErrorCode = axAPI.CommRqData(param.RQName, param.TrCode, param.PrevNext, param.ScreenNo);
        }
        private void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            SendAccount?.Invoke(this, new Account(axAPI.GetLoginInfo("ACCLIST")));
            string exclusion, date = GetDistinctDate(CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now.AddDays(1 - DateTime.Now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1);
            List<string> code = new List<string>
            {
                axAPI.GetFutureCodeByIndex(e.nErrCode)
            };
            for (int i = 2; i < 4; i++)
                foreach (var om in axAPI.GetActPriceList().Split(';'))
                {
                    exclusion = axAPI.GetOptionCode(om.Insert(3, "."), i, date);

                    if (code.Exists(o => o.Equals(exclusion)))
                        continue;

                    code.Add(exclusion);
                }
            code.RemoveAt(1);
            Delay.delay = 605;

            foreach (string output in code)
                RemainingDay(output);

            axAPI.KOA_Functions("ShowAccountWindow", "");
        }
        private void OnReceiveTrData(object sender, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            
        }
        private void OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
        }
        private void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
        }
        private void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
        }
        private int ErrorCode
        {
            get; set;
        }
        private ConnectAPI()
        {
            request = Delay.GetInstance(205);
            request.Run();
        }
        private AxKHOpenAPI axAPI;
        private readonly Delay request;
        private static ConnectAPI api;
        public event EventHandler<Account> SendAccount;
    }
}