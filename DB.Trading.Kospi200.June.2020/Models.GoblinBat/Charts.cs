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
        [Key, Range(2, 20), Column(Order = 3)]
        public int Short
        {
            get; set;
        }
        [Key, Range(5, 120), Column(Order = 4)]
        public int Long
        {
            get; set;
        }
        [Key, StringLength(6), Column(Order = 5)]
        public string Date
        {
            get; set;
        }
        [Required]
        public double ShortValue
        {
            get; set;
        }
        [Required]
        public double LongValue
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