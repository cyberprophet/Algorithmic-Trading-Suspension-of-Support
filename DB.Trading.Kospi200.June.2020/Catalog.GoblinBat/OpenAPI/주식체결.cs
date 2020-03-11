using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 주식체결 : IEnumerable
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
                    27,
                    28,
                    15,
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
                    228,
                    311,
                    290,
                    691,
                    567,
                    568
                };
            }
        }
    }
}