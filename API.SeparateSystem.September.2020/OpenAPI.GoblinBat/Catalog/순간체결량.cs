using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 순간체결량 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[]
            {
                20,
                9001,
                302,
                10,
                25,
                11,
                12,
                15,
                13,
                30
            })
                yield return index;
        }
    }
}