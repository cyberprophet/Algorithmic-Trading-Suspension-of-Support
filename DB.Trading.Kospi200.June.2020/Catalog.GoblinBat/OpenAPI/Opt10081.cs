using System;
using System.Collections;

namespace ShareInvest.Catalog
{
    public class Opt10081 : ScreenNumber, ITRs, IEnumerable
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
        readonly string[] output = { "현재가", "거래량", "일자", "종목코드", "거래대금", "시가", "고가", "저가", "수정주가구분", "수정비율", "대업종구분", "소업종구분", "종목정보", "수정주가이벤트", "전일종가" };
        const string code = "opt10081";
        const string id = "종목코드;기준일자;수정주가구분";
    }
}