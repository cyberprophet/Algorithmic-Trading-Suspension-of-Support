using System.Collections;

namespace ShareInvest.Catalog
{
    public class ETF_NAV : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
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
        };
    }
}