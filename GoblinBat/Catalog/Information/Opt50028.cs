using System.Collections;

namespace ShareInvest.Catalog
{
    public class Opt50028 : TR, IEnumerable
    {
        public Opt50028()
        {
            ID = "종목코드;시간단위";
            TrCode = "opt50028";
            ScreenNo = "5028";
        }
        public override string ID
        {
            get; protected set;
        }
        public override string Value
        {
            get
            {
                return string.Concat(code, ";1");
            }
            set
            {
                code = value;
            }
        }
        public override string RQName
        {
            get; set;
        }
        public override string TrCode
        {
            get; protected set;
        }
        public override int PrevNext
        {
            get; set;
        }
        public override string ScreenNo
        {
            get; protected set;
        }
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
            "전일종가"
        };
        private string code;
    }
}