using System;

namespace ShareInvest.Interface.OpenAPI
{
    public interface ISendSecuritiesAPI<T>
    {
        event EventHandler<T> Send;
    }
    public enum OrderType
    {
        신규매수 = 1,
        신규매도 = 2,
        매수취소 = 3,
        매도취소 = 4,
        매수정정 = 5,
        매도정정 = 6,
        예약매수 = 7,
        예약매도 = 8
    }
    public enum HogaGb
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