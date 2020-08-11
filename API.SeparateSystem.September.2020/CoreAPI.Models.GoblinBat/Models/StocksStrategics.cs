using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class StocksStrategics
    {
        [ForeignKey("Codes"), Column(Order = 2), MinLength(6), MaxLength(8)]
        public string Code
        {
            get; set;
        }
        [ForeignKey("CatalogStrategics"), Column(Order = 1)]
        public string Strategics
        {
            get; set;
        }
        [Required, Column(Order = 3), StringLength(6)]
        public string Date
        {
            get; set;
        }
        [Column(Order = 4), Range(1, long.MaxValue)]
        public long MaximumInvestment
        {
            get; set;
        }
        [Column(Order = 5)]
        public double CumulativeReturn
        {
            get; set;
        }
        [Column(Order = 6)]
        public double WeightedAverageDailyReturn
        {
            get; set;
        }
        [Column(Order = 7)]
        public double DiscrepancyRateFromExpectedStockPrice
        {
            get; set;
        }
    }
}