using System.Collections;

namespace ShareInvest.Catalog
{
    public struct Unspecified : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return "Error";
        }
    }
}