using System.ComponentModel.DataAnnotations;

namespace ShareInvest.Models
{
    public class Stocks
    {
        [Key, MaxLength(6)]
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
        public uint Price
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