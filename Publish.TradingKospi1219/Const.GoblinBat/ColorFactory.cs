using System.Drawing;
using ShareInvest.EventHandler;

namespace ShareInvest.Const
{
    public class ColorFactory
    {
        public Color Recent
        {
            get; private set;
        }
        public Color Weekly
        {
            get; private set;
        }
        public Color Biweekly
        {
            get; private set;
        }
        public Color Monthly
        {
            get; private set;
        }
        public Color For3Months
        {
            get; private set;
        }
        public Color Cumulative
        {
            get; private set;
        }
        public void OnReceiveColor(object sender, Statistics e)
        {
            switch (e.Name)
            {
                case 2:
                    Recent = e.Division;
                    StrRecent = string.Concat("_R", e.Check.ToString("P2"));
                    break;

                case 6:
                    Weekly = e.Division;
                    StrWeekly = string.Concat("_W", e.Check.ToString("P2"));
                    break;

                case 11:
                    Biweekly = e.Division;
                    StrBiweekly = string.Concat("_B", e.Check.ToString("P2"));
                    break;

                case 21:
                    Monthly = e.Division;
                    StrMonthly = string.Concat("_M", e.Check.ToString("P2"));
                    break;

                case 61:
                    For3Months = e.Division;
                    StrFor3Months = string.Concat("_F", e.Check.ToString("P2"));
                    break;

                case 1:
                    Cumulative = e.Division;
                    StrCumulative = string.Concat("_C", e.Check.ToString("P2"));
                    break;
            }
        }
        public string StrRecent
        {
            get; private set;
        }
        public string StrWeekly
        {
            get; private set;
        }
        public string StrBiweekly
        {
            get; private set;
        }
        public string StrMonthly
        {
            get; private set;
        }
        public string StrFor3Months
        {
            get; private set;
        }
        public string StrCumulative
        {
            get; private set;
        }
    }
}