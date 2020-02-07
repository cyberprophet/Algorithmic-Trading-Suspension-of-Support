using System.Collections;
using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
    public class Opt10079 : ScreenNumber, ITR, IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly string[] output =
        {
            "현재가",
            "거래량",
            "체결시간",
            "시가",
            "고가",
            "저가",
            "수정주가구분",
            "수정비율",
            "대업종구분",
            "소업종구분",
            "종목정보",
            "수정주가이벤트",
            "전일종가"
        };
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
                return string.Concat(Code, ";1;1");
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
        private string Code
        {
            get; set;
        }
        private const string code = "opt10079";
        private const string id = "종목코드;틱범위;수정주가구분";
    }
}