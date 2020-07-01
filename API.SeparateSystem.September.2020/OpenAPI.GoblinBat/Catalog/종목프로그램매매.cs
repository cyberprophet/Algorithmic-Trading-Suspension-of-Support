using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 종목프로그램매매 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[]
            {
                20,
                10,
                25,
                11,
                12,
                13,
                202,
                204,
                206,
                208,
                210,
                212,
                213,
                214,
                215,
                216
            })
                yield return index;
        }
    }
}