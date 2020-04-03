using System.Collections;

namespace ShareInvest.Catalog
{
    public struct CodeListByMarket : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var str in new string[]
            {
                "3",
                "8",
                "50",
                "4",
                "5",
                "6",
                "9",
                "30"
            })
                yield return str;
        }
    }
}