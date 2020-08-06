using System.Collections;

namespace ShareInvest.OpenAPI.Catalog
{
    public struct 주식예상체결 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[] { 20, 10, 11, 12, 15, 13, 25 })
                yield return index;
        }
    }
}