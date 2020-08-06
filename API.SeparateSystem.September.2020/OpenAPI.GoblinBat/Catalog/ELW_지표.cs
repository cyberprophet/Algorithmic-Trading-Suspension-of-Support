using System.Collections;

namespace ShareInvest.OpenAPI.Catalog
{
    public struct ELW_지표 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[]
            {
                20,
                666,
                1211,
                667,
                668,
                669
            })
                yield return index;
        }
    }
}