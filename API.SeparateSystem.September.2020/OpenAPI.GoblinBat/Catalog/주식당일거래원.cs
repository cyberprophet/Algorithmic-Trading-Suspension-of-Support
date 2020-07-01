using System.Collections;

namespace ShareInvest.Catalog
{
    public struct 주식당일거래원 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var index in new int[] { 141, 161, 166, 146, 271, 151, 171, 176, 156, 281, 142, 162, 167, 147, 272, 152, 172, 177, 157, 282, 143, 163, 168, 148, 273, 153, 173, 178, 158, 283, 144, 164, 169, 149, 274, 154, 174, 179, 159, 284, 145, 165, 170, 150, 275, 155, 175, 180, 160, 285, 261, 262, 263, 264, 267, 268, 337 })
                yield return index;
        }
    }
}