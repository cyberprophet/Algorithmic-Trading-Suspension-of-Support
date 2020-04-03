using System.Collections;

namespace ShareInvest.Catalog
{
    public struct ELW_이론가 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[]
            {
                20,
                10,
                670,
                671,
                672,
                673,
                674,
                675,
                676,
                706
            })
                yield return index;
        }
    }
}