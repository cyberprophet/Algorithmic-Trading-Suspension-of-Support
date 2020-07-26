using System;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI.Catalog
{
    class OPT50010 : TR
    {
        internal override void OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            var temp = base.OnReceiveTrData(single, multi, e);

            if (temp.Item1 != null)
                foreach (var str in temp.Item1)
                    Send?.Invoke(this, new SendSecuritiesAPI(str));

            while (temp.Item2?.Count > 0)
                foreach (var pop in temp.Item2.Dequeue())
                    SendMessage(code, pop);
        }
        protected internal override string LookupScreenNo
        {
            get
            {
                if (Count++ == 0x63)
                    Count = 0;

                return (0x1388 + (Count == 0 ? ++Screen : Screen)).ToString("D4");
            }
        }
        internal override string ID => id;
        internal override string Value
        {
            get; set;
        }
        internal override string RQName
        {
            get; set;
        } = name;
        internal override string TrCode => code;
        internal override int PrevNext
        {
            get; set;
        }
        internal override string ScreenNo => LookupScreenNo;
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        const string id = "종목코드;시간검색";
        const string name = "선옵호가잔량추이요청";
        const string code = "OPT50010";
        readonly string[] single = { "시가", "고가", "저가" };
        readonly string[] multi = { "호가시간", "매도호가수량", "최우선매도호가", "매수호가수량", "최우선매수호가", "호가순잔량", "매도호가총잔량", "매수호가총잔량", "순매수잔량" };
        public override event EventHandler<SendSecuritiesAPI> Send;
    }
}