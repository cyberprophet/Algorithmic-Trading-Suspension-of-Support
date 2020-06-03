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
        [Required, MinLength(6), MaxLength(8)]
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
        public string Strategy
        {
            get; set;
        }
        [Required]
        public CheckState RollOver
        {
            get; set;
        }
        [Required, Range(2, 20)]
        public int BaseShort
        {
            get; set;
        }
        [Required, Range(5, 120)]
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
        [Required, Range(1, 90)]
        public int MonoTime
        {
            get; set;
        }
        [Required, Range(2, 20)]
        public int MonoShort
        {
            get; set;
        }
        [Required, Range(5, 120)]
        public int MonoLong
        {
            get; set;
        }
    }
}