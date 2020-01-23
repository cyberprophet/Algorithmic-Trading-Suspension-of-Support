using System.Collections;

namespace ShareInvest.Catalog
{
    public class ELW_이론가 : IEnumerable
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
            670,
            671,
            672,
            673,
            674,
            675,
            676,
            706
        };
    }
}