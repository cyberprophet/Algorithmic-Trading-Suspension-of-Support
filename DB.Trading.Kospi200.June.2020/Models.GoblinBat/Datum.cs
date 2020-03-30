using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Datum
    {
        [Key, Column(Order = 1), StringLength(8)]
        public string Code
        {
            get; set;
        }
        [Key, Column(Order = 2), StringLength(15)]
        public string Date
        {
            get; set;
        }
        [Column(Order = 3)]
        public string Price
        {
            get; set;
        }
        [Column(Order = 4)]
        public string Volume
        {
            get; set;
        }
        [Column(Order = 5)]
        public string SellPrice
        {
            get; set;
        }
        [Column(Order = 6)]
        public string SellQuantity
        {
            get; set;
        }
        [Column(Order = 7)]
        public string TotalSellAmount
        {
            get; set;
        }
        [Column(Order = 8)]
        public string BuyPrice
        {
            get; set;
        }
        [Column(Order = 9)]
        public string BuyQuantity
        {
            get; set;
        }
        [Column(Order = 10)]
        public string TotalBuyAmount
        {
            get; set;
        }
        [ForeignKey("Code")]
        public virtual Codes Codes
        {
            get; set;
        }
    }
}