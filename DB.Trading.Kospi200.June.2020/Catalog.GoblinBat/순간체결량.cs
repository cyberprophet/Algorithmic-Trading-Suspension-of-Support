using System.Collections;

namespace ShareInvest.Catalog
{
    public class 순간체결량 : IEnumerable
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
            9001,
            302,
            10,
            25,
            11,
            12,
            15,
            13,
            30
        };
    }
}