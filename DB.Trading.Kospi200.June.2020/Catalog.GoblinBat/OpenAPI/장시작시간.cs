using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 장시작시간 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[]
            {
                215,
                20,
                214
            })
                yield return index;
        }
    }
}