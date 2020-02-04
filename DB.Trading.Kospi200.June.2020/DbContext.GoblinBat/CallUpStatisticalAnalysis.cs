using System.Collections;

namespace ShareInvest.GoblinBatContext
{
    public class CallUpStatisticalAnalysis : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            using (var db = new GoblinBatDbContext('1'))
            {
                foreach (var code in db.Codes)
                    yield return code;
            }
        }
    }
}