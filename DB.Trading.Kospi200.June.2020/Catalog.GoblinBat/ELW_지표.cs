using System.Collections;

namespace ShareInvest.Catalog
{
    public class ELW_지표 : IEnumerable
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
            666,
            1211,
            667,
            668,
            669
        };
    }
}