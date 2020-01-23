using System.Collections;

namespace ShareInvest.Catalog
{
    public class 파생실시간상하한 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
        {
            1365,
            1366,
            305,
            306
        };
    }
}