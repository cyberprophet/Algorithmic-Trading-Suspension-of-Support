using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 주식시간외호가 : IEnumerable
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
                    21,
                    131,
                    132,
                    135,
                    136
                };
            }
        }
    }
}