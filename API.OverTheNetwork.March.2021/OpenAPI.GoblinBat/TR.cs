using System;

using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI
{
    abstract class TR : ISendSecuritiesAPI<SendSecuritiesAPI>
    {
        public abstract event EventHandler<SendSecuritiesAPI> Send;
    }
    enum Market
    {
        장내 = 0,
        코스닥 = 10,
        ELW = 3,
        ETF = 8,
        KONEX = 50,
        뮤추얼펀드 = 4,
        신주인수권 = 5,
        리츠 = 6,
        하이얼펀드 = 9,
        K_OTC = 30
    }
}