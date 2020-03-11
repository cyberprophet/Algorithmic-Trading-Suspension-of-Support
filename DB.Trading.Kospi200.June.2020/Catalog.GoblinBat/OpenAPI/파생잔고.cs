using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 파생잔고 : IEnumerable
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
                    9201,
                    9001,
                    302,
                    10,
                    930,
                    931,
                    932,
                    933,
                    945,
                    946,
                    950,
                    951,
                    27,
                    28,
                    307,
                    8019,
                    397,
                    305,
                    306
                };
            }
        }
    }
}