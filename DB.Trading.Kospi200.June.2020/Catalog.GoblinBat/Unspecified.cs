using System.Collections;

namespace ShareInvest.Catalog
{
    public class Unspecified : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return "Error";
        }
    }
}