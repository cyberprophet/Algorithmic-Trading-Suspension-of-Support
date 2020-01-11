using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using AxKHOpenAPILib;
using ShareInvest.Catalog;
using ShareInvest.DelayRequest;
using ShareInvest.Interface;

namespace ShareInvest.OpenAPI
{
    public class ConnectAPI : AuxiliaryFunction
    {
        public void SetAPI(AxKHOpenAPI axAPI)
        {
            API = axAPI;
            axAPI.OnEventConnect += OnEventConnect;
            axAPI.OnReceiveTrData += OnReceiveTrData;
            axAPI.OnReceiveRealData += OnReceiveRealData;
            axAPI.OnReceiveMsg += OnReceiveMsg;
        }
        public void StartProgress()
        {
            if (API != null)
            {
                API.CommConnect();

                return;
            }
            Environment.Exit(0);
        }
        public static ConnectAPI GetInstance()
        {
            if (api == null)
                api = new ConnectAPI();

            return api;
        }
        private void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
        }
        private void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
        }
        private void OnReceiveTrData(object sender, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if (e.sTrCode.Contains("KOA"))
                return;

            Sb = new StringBuilder(512);
            int i, cnt = API.GetRepeatCnt(e.sTrCode, e.sRQName);

            for (i = 0; i < (cnt > 0 ? cnt : cnt + 1); i++)
            {
                foreach (string item in Array.Find(catalog, o => o.ToString().Contains(e.sTrCode.Substring(1))))
                    Sb.Append(API.GetCommData(e.sTrCode, e.sRQName, i, item).Trim()).Append(';');

                if (cnt > 0)
                    Sb.Append("*");
            }
            switch (Array.FindIndex(catalog, o => o.ToString().Contains(e.sTrCode.Substring(1))))
            {
                case 0:
                    FixUp(Sb.ToString().Split(';'), e.sRQName);
                    break;
            }
        }
        private void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            int i;
            string exclusion, date = GetDistinctDate(CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now.AddDays(1 - DateTime.Now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1);
            List<string> code = new List<string>
            {
                API.GetFutureCodeByIndex(e.nErrCode)
            };
            for (i = 2; i < 4; i++)
                foreach (var om in API.GetActPriceList().Split(';'))
                {
                    exclusion = API.GetOptionCode(om.Insert(3, "."), i, date);

                    if (code.Exists(o => o.Equals(exclusion)))
                        continue;

                    code.Add(exclusion);
                }
            code.RemoveAt(1);

            foreach (string output in code)
                RemainingDay(output);
        }
        private void FixUp(string[] param, string code)
        {
            SetInsertCode(code, param[72], param[63]);

            if (code.Contains("101") && param[63].Equals(DateTime.Now.ToString("yyyyMMdd")))
                RemainingDate = param[63];
        }
        private void RemainingDay(string code)
        {
            request.RequestTrData(new Task(() => InputValueRqData(new Opt50001 { Value = code, RQName = code })));
        }
        private void InputValueRqData(ITR param)
        {
            string[] count = param.ID.Split(';'), value = param.Value.Split(';');
            int i, l = count.Length;

            for (i = 0; i < l; i++)
                API.SetInputValue(count[i], value[i]);

            ErrorCode = API.CommRqData(param.RQName, param.TrCode, param.PrevNext, param.ScreenNo);

            if (ErrorCode < 0)
                new Error(ErrorCode);
        }
        private int ErrorCode
        {
            get; set;
        }
        private bool DeadLine
        {
            get; set;
        }
        private string RemainingDate
        {
            get; set;
        }
        private ConnectAPI()
        {
            request = Delay.GetInstance(605);
            request.Run();
        }
        private StringBuilder Sb
        {
            get; set;
        }
        private AxKHOpenAPI API
        {
            get; set;
        }
        private static ConnectAPI api;
        private readonly Delay request;
    }
}