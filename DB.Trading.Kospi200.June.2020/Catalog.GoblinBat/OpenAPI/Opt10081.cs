using System;
using System.Collections;

namespace ShareInvest.Catalog
{
    public class Opt10081 : ScreenNumber, ITR, IEnumerable
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
            "일자",
            "종목코드",
            "거래대금",
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
                return string.Concat(Code, ";", DateTime.Now.AddDays(-1).ToString("yyyyMMdd"), ";1");
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
        private const string code = "opt10081";
        private const string id = "종목코드;기준일자;수정주가구분";
    }
}