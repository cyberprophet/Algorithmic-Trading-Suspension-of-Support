using System;
using System.Collections;

namespace ShareInvest.Catalog
{
    public class Unspecified : IEnumerable
    {
        public IEnumerator GetEnumerator() => throw new NotImplementedException();
    }
}