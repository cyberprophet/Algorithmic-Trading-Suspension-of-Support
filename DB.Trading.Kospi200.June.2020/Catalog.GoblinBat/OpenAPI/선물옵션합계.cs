using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 선물옵션합계 : IEnumerable
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
                    13010,
                    13011,
                    13012,
                    13013,
                    13014,
                    13015,
                    13016,
                    13017,
                    13018,
                    13019,
                    13020,
                    13021,
                    13022,
                    13023,
                    13024,
                    13025,
                    13026,
                    13027,
                    13028,
                    13029
                };
            }
        }
    }
}