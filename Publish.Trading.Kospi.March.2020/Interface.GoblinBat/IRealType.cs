using System.Collections;

namespace ShareInvest.Interface
{
    public interface IRealType
    {
        enum RealType
        {
            선물옵션우선호가 = 9,
            선물시세 = 1,
            선물호가잔량 = 2,
            선물이론가 = 3,
            옵션시세 = 8,
            옵션호가잔량 = 5,
            옵션이론가 = 6,
            장시작시간 = 7,
            주문체결 = 0,
            파생잔고 = 4,
            주식예상체결 = 10,
            파생실시간상하한 = 11
        }
        IEnumerable[] Catalog
        {
            get;
        }
    }
}