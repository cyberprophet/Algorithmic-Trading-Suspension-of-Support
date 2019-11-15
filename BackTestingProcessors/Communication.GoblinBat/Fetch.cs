using System.Collections;

namespace ShareInvest.Communication
{
    public class Fetch : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (string val in fetch.DayChart)
                yield return val;

            foreach (string val in fetch.TickChart)
                yield return val;
        }
        private readonly IFetch fetch = Retrieve.Get();
    }
}