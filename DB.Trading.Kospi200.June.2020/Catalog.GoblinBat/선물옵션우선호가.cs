using System.Collections;

namespace ShareInvest.Catalog
{
    public class 선물옵션우선호가 : IEnumerable
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
            27,
            28
        };
    }
}