using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class EstimatedPrice
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
        [Column(Order = 4)]
        public double FirstQuarter
        {
            get; set;
        }
        [Column(Order = 5)]
        public double SecondQuarter
        {
            get; set;
        }
        [Column(Order = 6)]
        public double ThirdQuarter
        {
            get; set;
        }
        [Column(Order = 7)]
        public double Quarter
        {
            get; set;
        }
        [Column(Order = 8)]
        public double TheNextYear
        {
            get; set;
        }
        [Column(Order = 9)]
        public double TheYearAfterNext
        {
            get; set;
        }
    }
}