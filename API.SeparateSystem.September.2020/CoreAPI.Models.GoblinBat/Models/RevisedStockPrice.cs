using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class RevisedStockPrice
    {
        [ForeignKey("Codes"), Column(Order = 1), MinLength(6), MaxLength(8)]
        public string Code
        {
            get; set;
        }
        [Column(Order = 2), StringLength(6)]
        public string Date
        {
            get; set;
        }
        [Column(Order = 3), Required]
        public string Rate
        {
            get; set;
        }
        [Column(Order = 4), Required]
        public string Price
        {
            get; set;
        }
        [Column(Order = 5)]
        public string Revise
        {
            get; set;
        }
        [Column(Order = 6)]
        public string Name
        {
            get; set;
        }
    }
}