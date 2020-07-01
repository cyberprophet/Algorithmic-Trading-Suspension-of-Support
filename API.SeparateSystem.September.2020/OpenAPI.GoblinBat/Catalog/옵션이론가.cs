using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 옵션이론가 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[]
            {
                195,
                182,
                186,
                190,
                191,
                193,
                192,
                194,
                181,
                246,
                247,
                248,
                187,
                188,
                189
            })
                yield return index;
        }
    }
}