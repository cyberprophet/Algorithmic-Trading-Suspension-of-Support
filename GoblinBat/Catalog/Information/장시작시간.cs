using System.Collections;

namespace ShareInvest.Catalog
{
    public class 장시작시간 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
        {
            215,
            20,
            214
        };
    }
}