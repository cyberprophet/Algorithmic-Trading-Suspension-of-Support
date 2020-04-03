using System.Collections;

namespace ShareInvest.Catalog
{
    public class OPW20007 : ScreenNumber, ITRs, IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        public string Value
        {
            get; set;
        }
        public string RQName
        {
            get
            {
                return name;
            }
            set
            {

            }
        }
        public int PrevNext
        {
            get; set;
        }
        public string ID => id;
        public string TrCode => code;
        public string ScreenNo => GetScreenNumber();
        readonly string[] output = { "종목코드", "종목명", "매도매수구분", "수량", "매입단가", "현재가", "평가손익", "청산가능수량", "약정금액", "평가금액" };
        const string id = "계좌번호;비밀번호;비밀번호입력매체구분";
        const string name = "선옵잔고현황정산가기준요청";
        const string code = "OPW20007";
    }
}