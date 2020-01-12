using System.ComponentModel.DataAnnotations;

namespace ShareInvest.Models
{
    public class Futures
    {
        [Key, MaxLength(8)]
        public string Code
        {
            get; set;
        }
        [Key]
        public ulong Date
        {
            get; set;
        }
        [Required]
        public double Price
        {
            get; set;
        }
        [Required]
        public int Volume
        {
            get; set;
        }
    }
}