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
        public string ID
        {
            get
            {
                return id;
            }
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
        public string TrCode
        {
            get
            {
                return code;
            }
        }
        public int PrevNext
        {
            get; set;
        }
        public string ScreenNo
        {
            get
            {
                return GetScreenNumber();
            }
        }
        private readonly string[] output =
        {
            "현재가",
            "거래량",
            "체결시간",
            "시가",
            "고가",
            "저가",
            "전일종가"
        };
        private string Code
        {
            get; set;
        }
        private const string code = "opt50066";
        private const string id = "종목코드;시간단위";
    }
}