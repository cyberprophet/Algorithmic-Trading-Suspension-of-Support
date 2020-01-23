using System.Collections;

namespace ShareInvest.Catalog
{
    public class 주식거래원 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
        {
            9001,
            9026,
            302,
            334,
            20,
            203,
            207,
            210,
            211,
            260,
            337,
            10,
            11,
            12,
            25
        };
    }
}