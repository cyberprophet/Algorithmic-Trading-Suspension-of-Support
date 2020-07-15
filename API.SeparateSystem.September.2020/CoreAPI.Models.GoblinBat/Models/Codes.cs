using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Codes
    {
        [Key, MinLength(6), MaxLength(8), Column(Order = 1)]
        public string Code
        {
            get; set;
        }
        [Required, Column(Order = 2)]
        public string Name
        {
            get; set;
        }
        [Required, Column(Order = 3)]
        public string MaturityMarketCap
        {
            get; set;
        }
        [Column(Order = 4)]
        public double MarginRate
        {
            get; set;
        }
        [ForeignKey("Code")]
        public virtual ICollection<Futures> Futures
        {
            get; set;
        }
        [ForeignKey("Code")]
        public virtual ICollection<Options> Options
        {
            get; set;
        }
        [ForeignKey("Code")]
        public virtual ICollection<Stocks> Stocks
        {
            get; set;
        }
        [ForeignKey("Code")]
        public virtual ICollection<Days> Days
        {
            get; set;
        }
        public Codes()
        {
            Days = new HashSet<Days>();
            Futures = new HashSet<Futures>();
            Options = new HashSet<Options>();
            Stocks = new HashSet<Stocks>();
        }
    }
}