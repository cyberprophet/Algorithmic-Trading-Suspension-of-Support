using System.Collections;

namespace ShareInvest.Catalog
{
    public class 업종등락 : IEnumerable
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
            252,
            251,
            253,
            255,
            254,
            13,
            14,
            10,
            11,
            12,
            256,
            257,
            25
        };
    }
}