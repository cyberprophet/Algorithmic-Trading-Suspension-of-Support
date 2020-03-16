using System.Collections.Generic;
using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.GoblinBatContext;

namespace ShareInvest.Strategy
{
    public partial class Retrieve : CallUpStatisticalAnalysis
    {
        public void SetInitialzeTheCode(string code)
        {
            if (Chart == null && Quotes == null)
            {
                Chart = GetChart(code);
                Quotes = GetQuotes(code);
            }
        }
        public void SetInitializeTheChart()
        {
            Chart.Clear();
            Quotes.Clear();
            Chart = null;
            Quotes = null;
        }
        internal protected static Queue<Chart> Chart
        {
            get; private set;
        }
        internal protected static Queue<Quotes> Quotes
        {
            get; private set;
        }
    }
}