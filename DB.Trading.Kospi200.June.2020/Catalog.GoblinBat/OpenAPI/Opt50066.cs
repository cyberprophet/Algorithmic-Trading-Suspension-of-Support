using System.Collections;

namespace ShareInvest.Catalog
{
    public class Opt50066 : ScreenNumber, ITRs, IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        public string Value
        {
            get
            {
                return string.Concat(Code, ";1");
            }
            set
            {
                Code = value;
            }
        }
        public string RQName
        {
            get; set;
        }
        public int PrevNext
        {
            get; set;
        }
        public string ID => id;
        public string TrCode => code;
        public string ScreenNo => GetScreenNumber();
        string Code
        {
            get; set;
        }
        readonly string[] output = { "현재가", "거래량", "체결시간", "시가", "고가", "저가", "전일종가" };
        const string code = "opt50066";
        const string id = "종목코드;시간단위";
    }
}