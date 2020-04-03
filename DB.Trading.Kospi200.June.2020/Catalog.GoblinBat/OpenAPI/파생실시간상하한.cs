using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 파생실시간상하한 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[] { 1365, 1366, 305, 306 })
                yield return index;
        }
    }
}