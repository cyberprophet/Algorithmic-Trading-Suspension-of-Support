using System.Collections;

namespace ShareInvest.Catalog
{
    public class 선물이론가 : IEnumerable
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
            184,
            183,
            186,
            181,
            185,
            246,
            247,
            248
        };
    }
}