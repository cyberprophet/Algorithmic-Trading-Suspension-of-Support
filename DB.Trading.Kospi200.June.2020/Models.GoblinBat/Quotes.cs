using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Quotes
    {
        [Key, Column(Order = 1), StringLength(8)]
        public string Code
        {
            get; set;
        }
        [Key, Column(Order = 2), StringLength(15)]
        public string Date
        {
            get; set;
        }
        [Required]
        public string Contents
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