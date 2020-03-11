using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 주식시세 : IEnumerable
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
                    10,
                    11,
                    12,
                    27,
                    28,
                    13,
                    14,
                    16,
                    17,
                    18,
                    25,
                    26,
                    29,
                    30,
                    31,
                    32,
                    311,
                    567,
                    568
                };
            }
        }
    }
}