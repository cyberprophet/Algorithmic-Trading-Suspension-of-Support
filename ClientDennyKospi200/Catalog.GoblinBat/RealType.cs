using System.Collections;
using ShareInvest.Interface;
using ShareInvest.Storage;

namespace ShareInvest.Catalog
{
    public class RealType : IRealType
    {
        public IEnumerable[] Catalog
        {
            get; private set;
        } =
        {
           new 주문체결(),
           new 선물시세(),
           new 선물호가잔량(),
           new 선물이론가(),
           new 파생잔고(),
           new 옵션호가잔량(),
           new 옵션이론가(),
           new 장시작시간(),
           new 옵션시세(),
           new 선물옵션우선호가()
        };
    }
}