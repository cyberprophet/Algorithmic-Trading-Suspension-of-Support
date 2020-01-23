using System.Collections;

namespace ShareInvest.Catalog
{
    public class 주식체결 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
        {
            20,
            10,
            11,
            12,
            27,
            28,
            15,
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
            228,
            311,
            290,
            691,
            567,
            568
        };
    }
}