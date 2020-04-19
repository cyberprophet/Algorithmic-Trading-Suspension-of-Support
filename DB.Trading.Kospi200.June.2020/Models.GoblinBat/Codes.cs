using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShareInvest.Models
{
    public class Codes
    {
        [Key, MaxLength(8), MinLength(6)]
        public string Code
        {
            get; set;
        }
        [Required, MaxLength(14)]
        public string Name
        {
            get; set;
        }
        [Required, MaxLength(8)]
        public string Info
        {
            get; set;
        }
        public virtual ICollection<Statistics> Statistics
        {
            get; set;
        }
        public Codes() => Statistics = new HashSet<Statistics>();
    }
}