using System.Collections;

namespace ShareInvest.OpenAPI.Catalog
{
    public struct 선물옵션우선호가 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[]
            {
                10,
                27,
                28
            })
                yield return index;
        }
    }
}