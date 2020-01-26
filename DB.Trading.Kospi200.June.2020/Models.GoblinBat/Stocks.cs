using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Stocks
    {
        [Key, Column(Order = 1), StringLength(6)]
        public string Code
        {
            get; set;
        }
        [Key, Column(Order = 2), Range(190101090000000, 990101090000000)]
        public long Date
        {
            get; set;
        }
        [Required]
        public int Price
        {
            get; set;
        }
        [Required]
        public int Volume
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