using System.Collections;

namespace ShareInvest.Catalog
{
    public class KOA_NORMAL_FO_MOD : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly string[] output =
        {
            "주문번호"
        };
    }
}