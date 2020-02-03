using System.Collections;

namespace ShareInvest.Catalog
{
    public class KOA_CREATE_FO_ORD : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly string[] output = { "주문번호" };
    }
}