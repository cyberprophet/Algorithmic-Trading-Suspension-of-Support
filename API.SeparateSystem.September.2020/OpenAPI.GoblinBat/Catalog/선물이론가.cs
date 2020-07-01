using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 선물이론가 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[]
            {
                195,
                182,
                184,
                183,
                186,
                181,
                185,
                246,
                247,
                248
            })
                yield return index;
        }
    }
}