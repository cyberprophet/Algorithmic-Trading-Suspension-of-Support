using System;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI.Catalog
{
    class OPW20010 : TR, ISendSecuritiesAPI<SendSecuritiesAPI>
    {
        internal override void OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            var temp = base.OnReceiveTrData(single, null, e);

            if (temp.Item1 != null)
                Send?.Invoke(this, new SendSecuritiesAPI(temp.Item1[0x12], temp.Item1[7]));
        }
        internal override string ID => id;
        internal override string Value
        {
            get; set;
        }
        internal override string RQName
        {
            get => name;
            set
            {

            }
        }
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
        const string id = "계좌번호;비밀번호;비밀번호입력매체구분";
        const string name = "선옵예탁금및증거금조회요청";
        const string code = "OPW20010";
        readonly string[] single
            = { "예탁총액", "예탁현금", "예탁대용", "증거금총액", "증거금현금", "증거금대용금", "주문가능총액", "주문가능현금", "주문가능대용금", "추가증거금총액", "추가증거금현금", "추가증거금대용금", "전일장종료예탁총액", "전일장종료예탁현금", "전일장종료예탁대용금", "인출가능총액", "인출가능현금", "인출가능대용금", "순자산금액", "익일예탁총액", "개장예탁총액", "선물정산차금", "선물청산손익", "선물평가손익", "선물약정금액", "옵션결제차금", "옵션청산손익", "옵션평가손익", "옵션약정금액" };
        public override event EventHandler<SendSecuritiesAPI> Send;
    }
}