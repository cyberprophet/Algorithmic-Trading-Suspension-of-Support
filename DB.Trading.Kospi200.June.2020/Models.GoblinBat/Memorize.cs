using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Memorize
    {
        [Key, Column(Order = 1)]
        public long Index
        {
            get; set;
        }
        [Key, Column(Order = 2), StringLength(6)]
        public string Date
        {
            get; set;
        }
        [Key, Column(Order = 3), MinLength(6), MaxLength(8)]
        public string Code
        {
            get; set;
        }
        [Column(Order = 4)]
        public string Unrealized
        {
            get; set;
        }
        [Column(Order = 5)]
        public string Revenue
        {
            get; set;
        }
        [Required, Column(Order = 7)]
        public string Cumulative
        {
            get; set;
        }
        [Column(Order = 6)]
        public string Commission
        {
            get; set;
        }
        [Required, Column(Order = 8)]
        public int Statistic
        {
            get; set;
        }
        [ForeignKey("Index")]
        public virtual Strategics Strategy
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