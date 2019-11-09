using System.Collections;
using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
    public class Opt50066 : ScreenNumber, ITR, IEnumerable
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
        } = "종목코드;시간단위";
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
            get; private set;
        } = "opt50066";
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
    }
}