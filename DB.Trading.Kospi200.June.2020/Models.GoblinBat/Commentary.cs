using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Commentary
    {
        [Key, Column(Order = 1), MinLength(6), MaxLength(8)]
        public string Code
        {
            get; set;
        }
        [Key, Column(Order = 2)]
        public long Assets
        {
            get; set;
        }
        [Key, Column(Order = 3), StringLength(6)]
        public string Date
        {
            get; set;
        }
        [Key, Column(Order = 4)]
        public string Strategy
        {
            get; set;
        }
        [Column(Order = 5)]
        public long Unrealized
        {
            get; set;
        }
        [Column(Order = 6)]
        public long Revenue
        {
            get; set;
        }
        [Column(Order = 7)]
        public long Cumulative
        {
            get; set;
        }
        [Column(Order = 8)]
        public long Commission
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