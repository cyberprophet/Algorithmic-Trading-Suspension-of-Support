using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Days
    {
        [Key, Column(Order = 1), MaxLength(8)]
        public string Code
        {
            get; set;
        }
        [Key, Column(Order = 2)]
        public int Date
        {
            get; set;
        }
        [Required]
        public double Price
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