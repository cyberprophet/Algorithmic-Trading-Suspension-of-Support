using System.Collections;

namespace ShareInvest.Catalog
{
    public struct KOA_NORMAL_BUY_KQ_ORD : IEnumerable
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