using System.Collections;
using ShareInvest.Communicate;
using ShareInvest.RetrieveInformation;

namespace ShareInvest.Chart
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
        public Fetch()
        {
            fetch = Retrieve.Get();
        }
        private readonly IFetch fetch;
    }
}