using System.Collections;

namespace ShareInvest.Catalog
{
    public class 임의연장정보 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
        {
            580,
            581,
            582,
            583,
            584,
            585,
            586,
            587,
            588,
            589
        };
    }
}