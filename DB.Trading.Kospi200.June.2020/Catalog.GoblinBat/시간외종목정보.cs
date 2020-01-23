using System.Collections;

namespace ShareInvest.Catalog
{
    public class 시간외종목정보 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
        {
            10297,
            10305,
            10306,
            10307
        };
    }
}