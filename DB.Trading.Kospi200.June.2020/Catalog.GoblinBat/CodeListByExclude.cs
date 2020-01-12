using System.Collections;

namespace ShareInvest.Catalog
{
    public class CodeListByExclude : IEnumerable
    {
        private readonly string[] exclude =
        {
            "우",
            "우B",
            ")",
            "호",
            "ETN",
            "스팩",
            " B"
        };
        public IEnumerator GetEnumerator()
        {
            int i, l = exclude.Length;

            for (i = 0; i < l; i++)
                yield return exclude[i];
        }
    }
}