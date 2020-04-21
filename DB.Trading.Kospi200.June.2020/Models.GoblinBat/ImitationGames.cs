using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class ImitationGames
    {
        [Key, StringLength(6), Column(Order = 1)]
        public string Date
        {
            get; set;
        }
        [Required, Column(Order = 2)]
        public long Unrealized
        {
            get; set;
        }
        [Required, Column(Order = 3)]
        public long Revenue
        {
            get; set;
        }
        [Required, Column(Order = 4)]
        public int Fees
        {
            get; set;
        }
        [Required, Column(Order = 5)]
        public long Cumulative
        {
            get; set;
        }
        [Required, Column(Order = 6)]
        public int Statistic
        {
            get; set;
        }
        [Required, Range(1440, 1440), Column(Order = 7)]
        public int BaseTime
        {
            get; set;
        }
        [Key, Range(2, 20), Column(Order = 8)]
        public int BaseShort
        {
            get; set;
        }
        [Key, Range(5, 120), Column(Order = 9)]
        public int BaseLong
        {
            get; set;
        }
        [Key, Range(9, 180), Column(Order = 10)]
        public int NonaTime
        {
            get; set;
        }
        [Key, Range(2, 20), Column(Order = 11)]
        public int NonaShort
        {
            get; set;
        }
        [Key, Range(5, 120), Column(Order = 12)]
        public int NonaLong
        {
            get; set;
        }
        [Key, Range(8, 150), Column(Order = 13)]
        public int OctaTime
        {
            get; set;
        }
        [Key, Range(2, 20), Column(Order = 14)]
        public int OctaShort
        {
            get; set;
        }
        [Key, Range(5, 120), Column(Order = 15)]
        public int OctaLong
        {
            get; set;
        }
        [Key, Range(7, 120), Column(Order = 16)]
        public int HeptaTime
        {
            get; set;
        }
        [Key, Range(2, 20), Column(Order = 17)]
        public int HeptaShort
        {
            get; set;
        }
        [Key, Range(5, 120), Column(Order = 18)]
        public int HeptaLong
        {
            get; set;
        }
        [Key, Range(6, 90), Column(Order = 19)]
        public int HexaTime
        {
            get; set;
        }
        [Key, Range(2, 20), Column(Order = 20)]
        public int HexaShort
        {
            get; set;
        }
        [Key, Range(5, 120), Column(Order = 21)]
        public int HexaLong
        {
            get; set;
        }
        [Key, Range(5, 60), Column(Order = 22)]
        public int PentaTime
        {
            get; set;
        }
        [Key, Range(2, 20), Column(Order = 23)]
        public int PentaShort
        {
            get; set;
        }
        [Key, Range(5, 120), Column(Order = 24)]
        public int PentaLong
        {
            get; set;
        }
        [Key, Range(4, 45), Column(Order = 25)]
        public int QuadTime
        {
            get; set;
        }
        [Key, Range(2, 20), Column(Order = 26)]
        public int QuadShort
        {
            get; set;
        }
        [Key, Range(5, 120), Column(Order = 27)]
        public int QuadLong
        {
            get; set;
        }
        [Key, Range(3, 30), Column(Order = 28)]
        public int TriTime
        {
            get; set;
        }
        [Key, Range(2, 20), Column(Order = 29)]
        public int TriShort
        {
            get; set;
        }
        [Key, Range(5, 120), Column(Order = 30)]
        public int TriLong
        {
            get; set;
        }
        [Key, Range(2, 15), Column(Order = 31)]
        public int DuoTime
        {
            get; set;
        }
        [Key, Range(2, 20), Column(Order = 32)]
        public int DuoShort
        {
            get; set;
        }
        [Key, Range(5, 120), Column(Order = 33)]
        public int DuoLong
        {
            get; set;
        }
        [Key, Range(1, 5), Column(Order = 34)]
        public int MonoTime
        {
            get; set;
        }
        [Key, Range(2, 20), Column(Order = 35)]
        public int MonoShort
        {
            get; set;
        }
        [Key, Range(5, 120), Column(Order = 36)]
        public int MonoLong
        {
            get; set;
        }
        [ForeignKey("Statistics"), MinLength(6), MaxLength(8), Column(Order = 38)]
        public string Code
        {
            get; set;
        }
        [ForeignKey("Statistics"), Range(90000000, long.MaxValue), Column(Order = 37)]
        public long Assets
        {
            get; set;
        }
        [ForeignKey("Statistics"), Range(0.000015, 0.00003), Column(Order = 39)]
        public double Commission
        {
            get; set;
        }
        [ForeignKey("Statistics"), Column(Order = 40)]
        public double MarginRate
        {
            get; set;
        }
        [ForeignKey("Statistics"), Key, Column(Order = 41)]
        public string Strategy
        {
            get; set;
        }
        [ForeignKey("Statistics"), Key, Column(Order = 42)]
        public bool RollOver
        {
            get; set;
        }
        [ForeignKey("Code, Assets, Commission, MarginRate, Strategy, RollOver")]
        public virtual Statistics Statistics
        {
            get; set;
        }
    }
}