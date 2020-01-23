using System.Collections;

namespace ShareInvest.Catalog
{
    public class 주식시세 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
        {
            10,
            11,
            12,
            27,
            28,
            13,
            14,
            16,
            17,
            18,
            25,
            26,
            29,
            30,
            31,
            32,
            311,
            567,
            568
        };
    }
}