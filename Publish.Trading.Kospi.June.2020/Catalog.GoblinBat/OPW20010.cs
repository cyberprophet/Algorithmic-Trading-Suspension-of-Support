using System.Collections;
using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
    public class OPW20010 : ScreenNumber, ITR, IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        public string ID
        {
            get; private set;
        } = "계좌번호;비밀번호;비밀번호입력매체구분";
        public string Value
        {
            get; set;
        }
        public string RQName
        {
            get; set;
        } = "선옵예탁금및증거금조회요청";
        public string TrCode
        {
            get; private set;
        } = "OPW20010";
        public int PrevNext
        {
            get; set;
        } = 0;
        public string ScreenNo
        {
            get
            {
                return GetScreenNumber();
            }
        }
        private readonly string[] output =
        {
            "예탁총액",
            "예탁현금",
            "예탁대용",
            "증거금총액",
            "증거금현금",
            "증거금대용금",
            "주문가능총액",
            "주문가능현금",
            "주문가능대용금",
            "추가증거금총액",
            "추가증거금현금",
            "추가증거금대용금",
            "전일장종료예탁총액",
            "전일장종료예탁현금",
            "전일장종료예탁대용금",
            "인출가능총액",
            "인출가능현금",
            "인출가능대용금",
            "순자산금액",
            "익일예탁총액",
            "개장예탁총액",
            "선물정산차금",
            "선물청산손익",
            "선물평가손익",
            "선물약정금액",
            "옵션결제차금",
            "옵션청산손익",
            "옵션평가손익",
            "옵션약정금액"
        };
    }
}