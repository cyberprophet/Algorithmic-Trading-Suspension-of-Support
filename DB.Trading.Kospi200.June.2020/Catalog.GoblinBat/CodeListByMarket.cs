using System.Collections;

namespace ShareInvest.Catalog
{
    public class CodeListByMarket : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = sMarket.Length;

            for (i = 0; i < l; i++)
                yield return sMarket[i];
        }
        private readonly string[] sMarket =
        {
            "3",
            "8",
            "50",
            "4",
            "5",
            "6",
            "9",
            "30"
        };
    }
}