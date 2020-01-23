using System.Collections;

namespace ShareInvest.Catalog
{
    public class 파생잔고 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
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