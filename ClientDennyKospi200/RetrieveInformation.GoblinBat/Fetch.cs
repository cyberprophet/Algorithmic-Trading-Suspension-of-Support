using System.Collections;

namespace ShareInvest.RetrieveInformation
{
    public class Fetch : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (string val in Retrieve.Get().DayChart)
                yield return val;

            foreach (string val in Retrieve.Get().TickChart)
                yield return val;
        }
    }
}