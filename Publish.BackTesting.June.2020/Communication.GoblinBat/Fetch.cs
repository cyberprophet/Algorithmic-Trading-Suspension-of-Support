using System.Collections;

namespace ShareInvest.Communication
{
    public class Fetch : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var chart = Retrieve.GetInstance(code).DayChart;

            if (chart.Count > 0)
                foreach (var temp in chart)
                    yield return temp;

            var tick = Retrieve.GetInstance(code).TickChart;

            if (tick.Count > 0)
                foreach (var temp in tick)
                    yield return temp;
        }
        public Fetch(string code)
        {
            this.code = code;
        }
        private readonly string code;
    }
}