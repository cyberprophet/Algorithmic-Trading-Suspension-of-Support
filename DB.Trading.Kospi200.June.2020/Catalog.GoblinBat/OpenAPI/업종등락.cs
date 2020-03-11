using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 업종등락 : IEnumerable
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
                    252,
                    251,
                    253,
                    255,
                    254,
                    13,
                    14,
                    10,
                    11,
                    12,
                    256,
                    257,
                    25
                };
            }
        }
    }
}