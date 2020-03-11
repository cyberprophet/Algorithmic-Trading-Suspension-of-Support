using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 주식거래원 : IEnumerable
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
                    9001,
                    9026,
                    302,
                    334,
                    20,
                    203,
                    207,
                    210,
                    211,
                    260,
                    337,
                    10,
                    11,
                    12,
                    25
                };
            }
        }
    }
}