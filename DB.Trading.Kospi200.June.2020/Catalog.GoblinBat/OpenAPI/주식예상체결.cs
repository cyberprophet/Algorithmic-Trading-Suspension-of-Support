using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 주식예상체결 : IEnumerable
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
                    20,
                    10,
                    11,
                    12,
                    15,
                    13,
                    25
                };
            }
        }
    }
}