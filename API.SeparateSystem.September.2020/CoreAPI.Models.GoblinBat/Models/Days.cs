using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Days
    {
        [ForeignKey("Codes"), Column(Order = 1), MinLength(6), MaxLength(8)]
        public string Code
        {
            get; set;
        }
        [Column(Order = 2), StringLength(8)]
        public string Date
        {
            get; set;
        }
        [Required, Column(Order = 3)]
        public string Price
        {
            get; set;
        }
        [NotMapped]
        public string Retention
        {
            get; set;
        }
    }
}