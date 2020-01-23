using System.Collections;

namespace ShareInvest.Catalog
{
    public class 주식종목정보 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
        {
            297,
            592,
            593,
            305,
            306,
            307,
            689,
            594,
            382,
            370,
            300
        };
    }
}