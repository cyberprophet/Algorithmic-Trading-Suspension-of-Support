using System.Collections;

namespace ShareInvest.Catalog
{
    class KOA_NORMAL_BUY_KP_ORD : IEnumerable
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