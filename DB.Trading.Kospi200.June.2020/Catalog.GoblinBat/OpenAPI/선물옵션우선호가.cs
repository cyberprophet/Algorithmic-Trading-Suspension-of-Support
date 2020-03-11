using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 선물옵션우선호가 : IEnumerable
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
                    27,
                    28
                };
            }
        }
    }
}