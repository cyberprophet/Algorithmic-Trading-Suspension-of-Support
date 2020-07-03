using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Statistics
    {
        [Key, ForeignKey("Codes"), MinLength(6), MaxLength(8), Column(Order = 2)]
        public string Code
        {
            get; set;
        }
        [Key, Range(30000000, long.MaxValue), Column(Order = 1)]
        public long Assets
        {
            get; set;
        }
        [Key, Range(0.000015, 0.00003), Column(Order = 3)]
        public double Commission
        {
            get; set;
        }
        [Key, Column(Order = 4)]
        public double MarginRate
        {
            get; set;
        }
        [Key, Column(Order = 5)]
        public string Strategy
        {
            get; set;
        }
        [Key, Column(Order = 6)]
        public bool RollOver
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