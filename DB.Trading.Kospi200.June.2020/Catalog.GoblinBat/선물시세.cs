using System.Collections;

namespace ShareInvest.Catalog
{
    public class 선물시세 : IEnumerable
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
            11,
            12,
            27,
            28,
            15,
            13,
            14,
            16,
            17,
            18,
            195,
            182,
            184,
            183,
            186,
            181,
            185,
            25,
            197,
            26,
            246,
            247,
            248,
            30,
            196,
            1365,
            1366,
            1367,
            305,
            306,
        };
    }
}