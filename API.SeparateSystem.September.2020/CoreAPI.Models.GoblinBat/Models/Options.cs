using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Options
    {
        [ForeignKey("Codes"), Column(Order = 1), StringLength(8)]
        public string Code
        {
            get; set;
        }
        [NotMapped]
        public string Retention
        {
            get; set;
        }
        [Column(Order = 2), StringLength(15)]
        public string Date
        {
            get; set;
        }
        [Required, Column(Order = 3)]
        public string Price
        {
            get; set;
        }
        [Required, Column(Order = 4)]
        public int Volume
        {
            get; set;
        }
    }
}