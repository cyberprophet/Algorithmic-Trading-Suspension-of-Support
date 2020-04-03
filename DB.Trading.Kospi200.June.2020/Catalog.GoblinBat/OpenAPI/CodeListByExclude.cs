using System.Collections;

namespace ShareInvest.Catalog
{
    public struct CodeListByExclude : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var str in new string[]
            {
                "우",
                "우B",
                ")",
                "호",
                "ETN",
                "스팩",
                " B"
            })
                yield return str;
        }
    }
}