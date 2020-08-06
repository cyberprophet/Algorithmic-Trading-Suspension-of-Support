using System.Collections;

namespace ShareInvest.OpenAPI.Catalog
{
    public struct ETF_NAV : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[]
            {
                36,
                37,
                38,
                39,
                20,
                10,
                11,
                12,
                13,
                25,
                667,
                668,
                669,
                265,
                266
            })
                yield return index;
        }
    }
}