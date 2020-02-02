using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Logs
    {
        [Key, Column(Order = 1), MinLength(6), MaxLength(8)]
        public string Code
        {
            get; set;
        }
        [Key, Column(Order = 2)]
        public string Strategy
        {
            get; set;
        }
        [Key, Column(Order = 3)]
        public long Assets
        {
            get; set;
        }
        [Key, Column(Order = 4)]
        public int Time
        {
            get; set;
        }
        [Key, Column(Order = 5)]
        public int Short
        {
            get; set;
        }
        [Key, Column(Order = 6)]
        public int Long
        {
            get; set;
        }
        [Key, Column(Order = 7), Range(100101, 991230)]
        public int Date
        {
            get; set;
        }
        [Column(Order = 8)]
        public long Unrealized
        {
            get; set;
        }
        [Column(Order = 9)]
        public long Revenue
        {
            get; set;
        }
        [Column(Order = 10)]
        public long Cumulative
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