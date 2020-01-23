using System.Collections;

namespace ShareInvest.Catalog
{
    public class 업종지수 : IEnumerable
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
            15,
            13,
            14,
            16,
            17,
            18,
            25,
            26
        };
    }
}