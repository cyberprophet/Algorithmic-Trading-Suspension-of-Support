using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 주식종목정보 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[] { 297, 592, 593, 305, 306, 307, 689, 594, 382, 370, 300 })
                yield return index;
        }
    }
}