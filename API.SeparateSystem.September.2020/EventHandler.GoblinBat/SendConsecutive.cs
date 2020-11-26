using System;

using ShareInvest.Catalog;

namespace ShareInvest.EventHandler
{
    public class SendConsecutive : EventArgs
    {
        public string Date
        {
            get; private set;
        }
        public dynamic Price
        {
            get; private set;
        }
        public int Volume
        {
            get; private set;
        }
        public SendConsecutive(Charts chart)
        {
            if (int.TryParse(chart.Price, out int stocks))
                Price = stocks;

            else if (double.TryParse(chart.Price, out double futures))
                Price = futures;

            Date = CheckTheSAT(chart.Date);
            Volume = chart.Volume;
        }
        public SendConsecutive(string date, int price, int volume)
        {
            Date = CheckTheSAT(date);
            Price = price;
            Volume = volume;
        }
        string CheckTheSAT(string date)
        {
            switch (date.Length)
            {
                case 6:
                    var now = DateTime.Now;

                    if (Array.Exists(this.sat, o => o.Equals(now.ToString("yyMMdd"))) && now.Hour < 0x12 && uint.TryParse(date, out uint sat))
                        return (sat - 0x2710).ToString("D6");

                    break;

                case 0xF:
                    if (Array.Exists(this.sat, o => o.Equals(date.Substring(0, 6))) && date.Substring(6, 2).CompareTo("18") < 0 && ulong.TryParse(date, out ulong convert))
                        return (convert - 0x989680).ToString("D15");

                    break;
            }
            return date;
        }
        readonly string[] sat = { "201203" };
    }
}