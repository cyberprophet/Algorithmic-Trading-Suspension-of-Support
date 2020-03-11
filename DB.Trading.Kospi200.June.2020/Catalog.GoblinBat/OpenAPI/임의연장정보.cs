using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 임의연장정보 : IEnumerable
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
                    580,
                    581,
                    582,
                    583,
                    584,
                    585,
                    586,
                    587,
                    588,
                    589
                };
            }
        }
    }
}