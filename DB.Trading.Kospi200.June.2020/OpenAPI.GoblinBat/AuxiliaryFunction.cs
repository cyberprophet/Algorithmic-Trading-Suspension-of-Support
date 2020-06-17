using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ShareInvest.Catalog;
using ShareInvest.GoblinBatContext;

namespace ShareInvest.OpenAPI
{
    public class AuxiliaryFunction : CallUp
    {
        static uint Accumulate
        {
            get; set;
        }
        static uint ScreenNumber
        {
            get; set;
        }
        static StringBuilder CodeStorage
        {
            get; set;
        }
        protected internal string GetDistinctDate(int usWeekNumber)
        {
            DayOfWeek dt = DateTime.Now.AddDays(1 - DateTime.Now.Day).DayOfWeek;
            int check = dt.Equals(DayOfWeek.Friday) || dt.Equals(DayOfWeek.Saturday) ? 3 : 2;

            return usWeekNumber > check || usWeekNumber == check && (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday) || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday)) ? DateTime.Now.AddMonths(1).ToString("yyyyMM") : DateTime.Now.ToString("yyyyMM");
        }
        protected internal CollectedInformation FixUp(string param)
        {
            var info = param.Split(';');

            return new CollectedInformation
            {
                Code = info[3].Substring(1),
                Name = info[4],
                Amount = uint.TryParse(info[6], out uint amount) ? amount : 0,
                Price = int.TryParse(info[7], out int price) ? price : 0,
                Purchase = uint.TryParse(info[8], out uint purchase) ? purchase : 0
            };
        }
        protected internal void FixUp(string[] info)
        {
            if (info[0].Length > 0)
                SetInsertCode(info[0], info[1], info[35]);
        }
        protected internal List<string> SetCodeStorage(string[] arr)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder(1024);
            List<string> inven = new List<string>(16);

            foreach (string code in arr)
                if (code.Length > 0)
                {
                    if (i++ % 100 == 99)
                    {
                        inven.Add(sb.Append(code).ToString());
                        sb = new StringBuilder(1024);

                        continue;
                    }
                    sb.Append(code).Append(";");
                }
            inven.Add(sb.Remove(sb.Length - 1, 1).ToString());

            return inven;
        }
        protected internal void SetCodeStorage(string code)
        {
            if (CodeStorage != null && CodeStorage.Length > 0)
                CodeStorage.Append(code).Append(';');

            else
                CodeStorage = new StringBuilder(code).Append(';');
        }
        protected internal int GetStartingPrice(int price, bool info)
        {
            switch (price)
            {
                case int n when n >= 0 && n < 1000:
                    return price;

                case int n when n >= 1000 && n < 5000:
                    return (price / 5 + 1) * 5;

                case int n when n >= 5000 && n < 10000:
                    return (price / 10 + 1) * 10;

                case int n when n >= 10000 && n < 50000:
                    return (price / 50 + 1) * 50;

                case int n when n >= 100000 && n < 500000 && info:
                    return (price / 500 + 1) * 500;

                case int n when n >= 500000 && info:
                    return (price / 1000 + 1) * 1000;

                default:
                    return (price / 100 + 1) * 100;
            }
        }
        protected internal int GetQuoteUnit(int price, bool info)
        {
            switch (price)
            {
                case int n when n >= 0 && n < 1000:
                    return 1;

                case int n when n >= 1000 && n < 5000:
                    return 5;

                case int n when n >= 5000 && n < 10000:
                    return 10;

                case int n when n >= 10000 && n < 50000:
                    return 50;

                case int n when n >= 100000 && n < 500000 && info:
                    return 500;

                case int n when n >= 500000 && info:
                    return 1000;

                default:
                    return 100;
            }
        }
        protected internal uint GetScreenNumber()
        {
            if (Accumulate++ > 99)
                Accumulate = 0;

            if (Accumulate == 0 && ++ScreenNumber > 99)
                ScreenNumber = 0;

            return ScreenNumber;
        }
        protected internal Tuple<int, string> CallUpStorage
        {
            get
            {
                if (CodeStorage != null && CodeStorage.Length > 0)
                {
                    var str = CodeStorage.Remove(CodeStorage.Length - 1, 1).ToString();

                    return new Tuple<int, string>(str.Split(';').Length, str);
                }
                return null;
            }
        }
        protected internal string SetPassword => password;
        protected internal string OnReceiveData => data;
        protected internal string TR => tr;
        protected internal string Failure => failure;
        protected internal string GoblinBat => goblin;
        protected internal string Collection => collection;
        protected internal string Response => response;
        protected internal StringBuilder Exists => new StringBuilder(exists);
        protected internal AuxiliaryFunction(string key) : base(key) => this.key = key;
        protected internal readonly IEnumerable[] catalogReal =
        {
            new 주문체결(),
            new 선물시세(),
            new 선물호가잔량(),
            new 선물이론가(),
            new 파생잔고(),
            new 옵션시세(),
            new 옵션호가잔량(),
            new 옵션이론가(),
            new 선물옵션우선호가(),
            new 장시작시간(),
            new 주식체결(),
            new 주식호가잔량(),
            new 주식예상체결(),
            new 주식우선호가(),
            new 주식당일거래원(),
            new 종목프로그램매매(),
            new 주식종목정보(),
            new 주식시세(),
            new 주식시간외호가(),
            new 파생실시간상하한(),
            new 시간외종목정보(),
            new ELW_이론가(),
            new ELW_지표(),
            new ETF_NAV(),
            new 선물옵션합계(),
            new 순간체결량(),
            new 업종등락(),
            new 업종지수(),
            new 임의연장정보(),
            new 주식거래원(),
            new 주식예상체결(),
            new 투자자별매매()
        };
        protected internal readonly IEnumerable[] catalogTR =
        {
            new Unspecified(),
            new Opt50028(),
            new Opt50066(),
            new Opt10079(),
            new Opt10081(),
            new Opt50001(),
            new OPTKWFID(),
            new KOA_CREATE_FO_ORD(),
            new KOA_NORMAL_FO_MOD(),
            new KOA_NORMAL_FO_CANCEL(),
            new OPW20010(),
            new OPW20007(),
            new Opw00005(),
            new KOA_NORMAL_BUY_KP_ORD(),
            new KOA_NORMAL_BUY_KQ_ORD(),
            new KOA_NORMAL_SELL_KP_ORD(),
            new KOA_NORMAL_SELL_KQ_ORD()
        };
        protected internal readonly string[] basic =
        {
            "모의투자 선물옵션 신규주문 완료",
            "모의투자 정상처리 되었습니다",
            "모의투자 정정/취소할 수량이 없습니다",
            "모의투자 주문가능 금액을 확인하세요",
            "모의투자 정정가격이 원주문가격과 같습니다",
            "모의투자 원주문번호가 존재하지 않습니다",
            "모의투자 주문처리가 안되었습니다(2)",
            "모의투자 서비스 지연입니다. 잠시후 재시도 바랍니다..",
            "모의투자 상하한가 오류"
        };
        protected internal readonly string[] exclude =
        {
            "115960",
            "006800",
            "001880",
            "072770"
        };
        protected internal readonly string[] message = { lookUp, end, bOrderComplete, sOrderComplete, look };
        protected internal readonly string key;
        protected internal const string format = "yyMMdd";
        protected internal const string market = "거래소";
        const string collection = "백테스팅에 필요한 자료를 수집합니다.";
        const string exists = "Information that already Exists";
        const string lookUp = "모의투자 조회가 완료되었습니다";
        const string failure = "전문 처리 실패(-22)";
        const string tr = "서비스 TR을 확인바랍니다.(0006)";
        const string data = "트레이딩에 필요한 자료를 수집합니다.\n\n잠시만 기다려주세요.";
        const string password = "비밀번호 설정을 하시겠습니까?\n\n자동로그인 기능을 사용하시면 편리합니다.";
        const string goblin = "GoblinBat";
        const string response = "응답이 지연되고 있습니다";
        const string end = "장종료되었습니다";
        const string bOrderComplete = "매수주문이 완료되었습니다.";
        const string sOrderComplete = "매도주문이 완료되었습니다.";
        const string look = "조회가 완료되었습니다";
    }
    enum RealType
    {
        주문체결 = 0,
        선물시세 = 1,
        선물호가잔량 = 2,
        선물이론가 = 3,
        파생잔고 = 4,
        옵션시세 = 5,
        옵션호가잔량 = 6,
        옵션이론가 = 7,
        선물옵션우선호가 = 8,
        장시작시간 = 9,
        주식체결 = 10,
        주식호가잔량 = 11,
        주식예상체결 = 12,
        주식우선호가 = 13,
        주식당일거래원 = 14,
        종목프로그램매매 = 15,
        주식종목정보 = 16,
        주식시세 = 17,
        주식시간외호가 = 18,
        파생실시간상하한 = 19,
        시간외종목정보 = 20
    }
    enum OrderType
    {
        신규매수 = 1,
        신규매도 = 2,
        매수취소 = 3,
        매도취소 = 4,
        매수정정 = 5,
        매도정정 = 6
    }
    enum HogaGb
    {
        지정가 = 00,
        시장가 = 03,
        조건부지정가 = 05,
        최유리지정가 = 06,
        최우선지정가 = 07,
        지정가IOC = 10,
        시장가IOC = 13,
        최유리IOC = 16,
        지정가FOK = 20,
        시장가FOK = 23,
        최유리FOK = 26,
        장전시간외종가 = 61,
        시간외단일가매매 = 62,
        장후시간외종가 = 81
    }
}