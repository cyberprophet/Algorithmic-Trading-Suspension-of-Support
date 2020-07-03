using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Strategics
    {
        [Key, StringLength(6), Column(Order = 1)]
        public string Date
        {
            get; set;
        }
        [Required]
        public long Unrealized
        {
            get; set;
        }
        [Required]
        public long Revenue
        {
            get; set;
        }
        [Required]
        public int Fees
        {
            get; set;
        }
        [Required]
        public long Cumulative
        {
            get; set;
        }
        [Required]
        public int Statistic
        {
            get; set;
        }
        [Required, Range(1440, 1440)]
        public int BaseTime
        {
            get; set;
        }
        [Key, Range(2, 20), Column(Order = 3)]
        public int BaseShort
        {
            get; set;
        }
        [Key, Range(5, 120), Column(Order = 4)]
        public int BaseLong
        {
            get; set;
        }
        [Required, Range(0, 180)]
        public int NonaTime
        {
            get; set;
        }
        [Required, Range(2, 20)]
        public int NonaShort
        {
            get; set;
        }
        [Required, Range(5, 120)]
        public int NonaLong
        {
            get; set;
        }
        [Required, Range(0, 150)]
        public int OctaTime
        {
            get; set;
        }
        [Required, Range(2, 20)]
        public int OctaShort
        {
            get; set;
        }
        [Required, Range(5, 120)]
        public int OctaLong
        {
            get; set;
        }
        [Required, Range(0, 120)]
        public int HeptaTime
        {
            get; set;
        }
        [Required, Range(2, 20)]
        public int HeptaShort
        {
            get; set;
        }
        [Required, Range(5, 120)]
        public int HeptaLong
        {
            get; set;
        }
        [Required, Range(0, 90)]
        public int HexaTime
        {
            get; set;
        }
        [Required, Range(2, 20)]
        public int HexaShort
        {
            get; set;
        }
        [Required, Range(5, 120)]
        public int HexaLong
        {
            get; set;
        }
        [Required, Range(0, 60)]
        public int PentaTime
        {
            get; set;
        }
        [Required, Range(2, 20)]
        public int PentaShort
        {
            get; set;
        }
        [Required, Range(5, 120)]
        public int PentaLong
        {
            get; set;
        }
        [Required, Range(0, 45)]
        public int QuadTime
        {
            get; set;
        }
        [Required, Range(2, 20)]
        public int QuadShort
        {
            get; set;
        }
        [Required, Range(5, 120)]
        public int QuadLong
        {
            get; set;
        }
        [Required, Range(0, 30)]
        public int TriTime
        {
            get; set;
        }
        [Required, Range(2, 20)]
        public int TriShort
        {
            get; set;
        }
        [Required, Range(5, 120)]
        public int TriLong
        {
            get; set;
        }
        [Required, Range(0, 15)]
        public int DuoTime
        {
            get; set;
        }
        [Required, Range(2, 20)]
        public int DuoShort
        {
            get; set;
        }
        [Required, Range(5, 120)]
        public int DuoLong
        {
            get; set;
        }
        [Key, Range(1, 90), Column(Order = 5)]
        public int MonoTime
        {
            get; set;
        }
        [Key, Range(2, 20), Column(Order = 6)]
        public int MonoShort
        {
            get; set;
        }
        [Key, Range(5, 120), Column(Order = 7)]
        public int MonoLong
        {
            get; set;
        }
        [Required, StringLength(8)]
        public string Code
        {
            get; set;
        }
        [Required, Range(30000000, long.MaxValue)]
        public long Assets
        {
            get; set;
        }
        [Required, Range(0.000015, 0.00003)]
        public double Commission
        {
            get; set;
        }
        [Required]
        public double MarginRate
        {
            get; set;
        }
        [Key, StringLength(2), Column(Order = 2)]
        public string Strategy
        {
            get; set;
        }
        [Required]
        public bool RollOver
        {
            get; set;
        }
        [Key, Column(Order = 8), MinLength(2), MaxLength(20)]
        public string Primary
        {
            get; set;
        }
    }
}