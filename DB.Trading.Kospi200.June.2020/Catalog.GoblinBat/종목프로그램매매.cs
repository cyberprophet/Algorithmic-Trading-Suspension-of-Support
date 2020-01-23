using System.Collections;

namespace ShareInvest.Catalog
{
    public class 종목프로그램매매 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
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
        };
    }
}