using System.Collections;
using ShareInvest.Storage;

namespace ShareInvest.OpenAPI
{
    internal class RealType
    {
        internal enum EnumType
        {
            선물시세 = 0,
            선물호가잔량 = 1,
            옵션시세 = 2,
            옵션호가잔량 = 3,
            주식체결 = 4,
            주식호가잔량 = 5,
            장시작시간 = 6,
            선물이론가 = 7,
            옵션이론가 = 8,
            선물옵션우선호가 = 9,
            주식예상체결 = 10,
            주식우선호가 = 11,
            주식당일거래원 = 12,
            종목프로그램매매 = 13,
            주식종목정보 = 14,
            주식시세 = 15,
            주식시간외호가 = 16,
            파생실시간상하한 = 17
        }
        internal readonly IEnumerable[] type =
        {
            new 선물시세(),
            new 선물호가잔량(),
            new 옵션시세(),
            new 옵션호가잔량(),
            new 주식체결(),
            new 주식호가잔량(),
            new 장시작시간(),
            new 선물이론가(),
            new 옵션이론가(),
            new 선물옵션우선호가(),
            new 주식예상체결(),
            new 주식우선호가(),
            new 주식당일거래원(),
            new 종목프로그램매매(),
            new 주식종목정보(),
            new 주식시세(),
            new 주식시간외호가(),
            new 파생실시간상하한(),
            new ELW_이론가(),
            new ELW_지표(),
            new ETF_NAV(),
            new 선물옵션합계(),
            new 순간체결량(),
            new 시간외종목정보(),
            new 업종등락(),
            new 업종지수(),
            new 임의연장정보(),
            new 주식거래원(),
            new 주식예상체결(),
            new 투자자별매매()
        };
    }
}