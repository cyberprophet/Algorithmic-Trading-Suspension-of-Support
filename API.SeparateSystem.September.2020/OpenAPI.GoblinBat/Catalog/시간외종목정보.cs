using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 시간외종목정보 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[]
            {
                10297,
                10305,
                10306,
                10307
            })
                yield return index;
        }
    }
}