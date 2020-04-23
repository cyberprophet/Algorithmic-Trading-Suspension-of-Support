using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Charts
    {
        [Key, MinLength(6), MaxLength(8), Column(Order = 1)]
        public string Code
        {
            get; set;
        }
        [Key, Range(1, 1440), Column(Order = 2)]
        public int Time
        {
            get; set;
        }
        [Key, Range(2, 120), Column(Order = 3)]
        public int Base
        {
            get; set;
        }
        [Key, MinLength(6), MaxLength(10), Column(Order = 4)]
        public string Date
        {
            get; set;
        }
        [Required]
        public double Value
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