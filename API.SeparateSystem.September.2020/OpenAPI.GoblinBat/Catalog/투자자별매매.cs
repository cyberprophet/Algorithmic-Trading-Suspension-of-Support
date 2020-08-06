using System.Collections;

namespace ShareInvest.OpenAPI.Catalog
{
    public struct 투자자별매매 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[] { 20, 217, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 561, 562 })
                yield return index;
        }
    }
}