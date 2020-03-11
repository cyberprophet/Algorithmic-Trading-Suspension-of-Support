using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 주식우선호가 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in Output)
                yield return index;
        }
        private int[] Output
        {
            get
            {
                return new int[]
                {
                    27,
                    28
                };
            }
        }
    }
}