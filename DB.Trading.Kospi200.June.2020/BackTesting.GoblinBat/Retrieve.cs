using System.Collections.Generic;
using ShareInvest.Catalog;
using ShareInvest.Catalog.XingAPI;
using ShareInvest.GoblinBatContext;

namespace ShareInvest.Strategy
{
    public partial class Retrieve : CallUpStatisticalAnalysis
    {
        public Retrieve(string key) : base(key)
        {

        }
        public void SetInitialzeTheCode(string code)
        {
            if (Chart == null && Quotes == null)
            {
                Chart = GetChart(code);
                Quotes = GetQuotes(code);
            }
        }
        public void SetInitialzeTheCode()
        {
            Code = GetStrategy();
            SetInitialzeTheCode(Code);
        }
        public void SetInitializeTheChart()
        {
            if (Chart != null)
            {
                Chart.Clear();
                Chart = null;
            }
            if (Quotes != null)
            {
                Quotes.Clear();
                Quotes = null;
            }
        }
        public static string Code
        {
            get; set;
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