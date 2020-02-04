using System.Collections;
using ShareInvest.Catalog;

namespace ShareInvest.OpenAPI
{
    internal class RealType
    {
        internal enum EnumType
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
        internal readonly IEnumerable[] type =
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
    }
}