using System.Collections;

namespace ShareInvest.Catalog
{
    public class Opw00005 : ScreenNumber, ITRs, IEnumerable
    {
        public string ID => id;
        public string Value
        {
            get; set;
        }
        public string RQName
        {
            get => name;
            set
            {

            }
        }
        public string TrCode => code;
        public int PrevNext
        {
            get; set;
        }
        public string ScreenNo => GetScreenNumber();
        public static bool Switch
        {
            get; set;
        }
        public IEnumerator GetEnumerator()
        {
            int i, l = (Switch ? opSingle : opMultiple).Length;

            for (i = 0; i < l; i++)
                yield return (Switch ? opSingle : opMultiple)[i];
        }
        const string code = "opw00005";
        const string name = "체결잔고요청";
        const string id = "계좌번호;비밀번호;비밀번호입력매체구분";
        readonly string[] opSingle = { "예수금", "예수금D+1", "예수금D+2", "출금가능금액", "미수확보금", "대용금", "권리대용금", "주문가능현금", "현금미수금", "신용이자미납금", "기타대여금", "미상환융자금", "증거금현금", "증거금대용", "주식매수총액", "평가금액합계", "총손익합계", "총손익률", "총재매수가능금액", "20주문가능금액", "30주문가능금액", "40주문가능금액", "50주문가능금액", "60주문가능금액", "100주문가능금액", "신용융자합계", "신용융자대주합계", "신용담보비율", "예탁담보대출금액", "매도담보대출금액", "조회건수" };
        readonly string[] opMultiple = { "신용구분", "대출일", "만기일", "종목번호", "종목명", "결제잔고", "현재잔고", "현재가", "매입단가", "매입금액", "평가금액", "평가손익", "손익률" };
    }
}