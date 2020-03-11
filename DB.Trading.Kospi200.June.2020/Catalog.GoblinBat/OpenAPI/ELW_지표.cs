using System.Collections;

namespace ShareInvest.Catalog
{
    public struct ELW_지표 : IEnumerable
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
                    666,
                    1211,
                    667,
                    668,
                    669
                };
            }
        }
    }
}