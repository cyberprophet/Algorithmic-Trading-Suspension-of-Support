using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class CatalogStrategics
    {
        [Key, Column(Order = 1)]
        public string Strategics
        {
            get; set;
        }
        [Required, Column(Order = 2)]
        public int Short
        {
            get; set;
        }
        [Required, Column(Order = 3)]
        public int Long
        {
            get; set;
        }
        [Column(Order = 4)]
        public int Trend
        {
            get; set;
        }
        [Column(Order = 5)]
        public double RealizeProfit
        {
            get; set;
        }
        [Column(Order = 6)]
        public double AdditionalPurchase
        {
            get; set;
        }
        [Column(Order = 7)]
        public int Quantity
        {
            get; set;
        }
        [Column(Order = 8)]
        public int QuoteUnit
        {
            get; set;
        }
        [Column(Order = 9), StringLength(1)]
        public string LongShort
        {
            get; set;
        }
        [Column(Order = 10), StringLength(1)]
        public string TrendType
        {
            get; set;
        }
        [Column(Order = 11), StringLength(1)]
        public string Setting
        {
            get; set;
        }
        [ForeignKey("Strategics")]
        public virtual ICollection<StocksStrategics> Stocks
        {
            get; set;
        }
        [ForeignKey("Strategics")]
        public virtual ICollection<EstimatedPrice> Consensus
        {
            get; set;
        }
        public CatalogStrategics()
        {
            Stocks = new HashSet<StocksStrategics>();
            Consensus = new HashSet<EstimatedPrice>();
        }
    }
}