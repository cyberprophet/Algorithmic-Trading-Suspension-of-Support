using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Forms;

namespace ShareInvest.Models
{
    public class Identify
    {
        [Key, StringLength(20), Column(Order = 1)]
        public string Identity
        {
            get; set;
        }
        [Key, StringLength(6), Column(Order = 2)]
        public string Date
        {
            get; set;
        }
        [MinLength(6), MaxLength(8)]
        public string Code
        {
            get; set;
        }
        [Range(90000000, long.MaxValue)]
        public long Assets
        {
            get; set;
        }
        [Range(0.000015, 0.00003)]
        public double Commission
        {
            get; set;
        }
        [Required]
        public string Strategy
        {
            get; set;
        }
        [Required]
        public CheckState RollOver
        {
            get; set;
        }
    }
}