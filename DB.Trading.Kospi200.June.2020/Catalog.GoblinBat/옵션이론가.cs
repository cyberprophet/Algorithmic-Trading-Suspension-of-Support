using System.Collections;

namespace ShareInvest.Catalog
{
    public class 옵션이론가 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
        {
            195,
            182,
            186,
            190,
            191,
            193,
            192,
            194,
            181,
            246,
            247,
            248,
            187,
            188,
            189
        };
    }
}