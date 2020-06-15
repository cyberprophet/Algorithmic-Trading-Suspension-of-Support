using System.Collections;

namespace ShareInvest.Catalog
{
    public struct KOA_NORMAL_SELL_KP_ORD : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var str in new string[]
            {
                "주문번호"
            })
                yield return str;
        }
    }
}