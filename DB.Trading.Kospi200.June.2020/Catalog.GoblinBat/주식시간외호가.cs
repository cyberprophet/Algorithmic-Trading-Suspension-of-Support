using System.Collections;

namespace ShareInvest.Catalog
{
    public class 주식시간외호가 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
        {
            21,
            131,
            132,
            135,
            136
        };
    }
}