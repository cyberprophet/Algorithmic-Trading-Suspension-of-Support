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
        public virtual ICollection<Days> Days
        {
            get; set;
        }
        public virtual ICollection<Futures> Futures
        {
            get; set;
        }
        public virtual ICollection<Options> Options
        {
            get; set;
        }
        public virtual ICollection<Stocks> Stocks
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