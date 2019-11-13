using System.Collections;
using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
    public class OPW20007 : ScreenNumber, ITR, IEnumerable
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
        } = "선옵잔고현황정산가기준요청";
        public string TrCode
        {
            get; private set;
        } = "OPW20007";
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
            "종목코드",
            "종목명",
            "매도매수구분",
            "수량",
            "매입단가",
            "현재가",
            "평가손익",
            "청산가능수량",
            "약정금액",
            "평가금액"
        };
    }
}