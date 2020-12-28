using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class IncorporatedStocks
    {
        [ForeignKey("Codes"), Key, MinLength(6), MaxLength(8), Column(Order = 1)]
        public string Code
        {
            get; set;
        }
        [Required, Column(Order = 2)]
        public string Name
        {
            get; set;
        }
        [Required, Column(Order = 3), StringLength(6)]
        public string Date
        {
            get; set;
        }
        [Column(Order = 4)]
        public int Capitalization
        {
            get; set;
        }
        [Required, Column(Order = 5), StringLength(1)]
        public string Market
        {
            get; set;
        }
    }
}